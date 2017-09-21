using Collab.Auth;
using Collab.Auth.Identity;
using Collab.Auth.Providers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Collab.Api.Config {
    public static class AuthConfig {
        // Create an extension method for the IServiceCollection
        public static void ConfigureAuthentication(this IServiceCollection services, IHostingEnvironment environment, IConfiguration config) {
            var authEnvironment = AuthenticationEnvironment.Development;

            if (environment.IsProduction())
                authEnvironment = AuthenticationEnvironment.Production;
            else if (environment.IsStaging())
                authEnvironment = AuthenticationEnvironment.Staging;

            // Register the core auth components
            services.AddSingleton(typeof(AuthenticationEnvironment), authEnvironment);
            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddSingleton<IIdentitySessionEventManager<HttpContext>, IdentitySessionEventManager<HttpContext>>();
            services.AddSingleton<IClaimsProvider, RoleClaimsProvider>();

            var cookieEventShim = new CookieEventShim();
            var authBuilder = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // TODO: This is a shithouse hack, but we can't resolve services at this point (the IIdentitySessionEventManager)
            // so we need to set the cookie 'Events' handler to a singleton and modify the EventManager property later.
            services.AddSingleton(cookieEventShim);

            authBuilder.AddCookie(opts => {
                opts.Cookie.HttpOnly = true;
                opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opts.Cookie.Name = ".token";
                opts.Events = cookieEventShim;
            });
        }

        public static void UseAuthentication(this IApplicationBuilder app) {
            var cookieEventShim = app.ApplicationServices.GetService<CookieEventShim>();
            var eventManager = app.ApplicationServices.GetService<IIdentitySessionEventManager<HttpContext>>();

            cookieEventShim.EventManager = eventManager;
        }

        private class CookieEventShim : CookieAuthenticationEvents {
            public IIdentitySessionEventManager<HttpContext> EventManager { get; set; }

            public override Task SigningIn(CookieSigningInContext context) {
                EventManager?.OnSigningIn(context.HttpContext);
                return Task.CompletedTask;
            }

            public override Task SignedIn(CookieSignedInContext context) {
                EventManager?.OnSignedIn(context.HttpContext);
                return Task.CompletedTask;
            }

            public override Task ValidatePrincipal(CookieValidatePrincipalContext context) {
                // Need to raise both events here, which isn't great but it's as fine-grain
                // as cookie middleware allows
                EventManager?.OnRestoring(context.HttpContext);
                EventManager?.OnRestored(context.HttpContext);

                return Task.CompletedTask;
            }

            public override Task SigningOut(CookieSigningOutContext context) {
                // Need to raise both events here, which isn't great but it's as fine-grain
                // as cookie middleware allows
                EventManager?.OnSigningOut(context.HttpContext);
                EventManager?.OnSignedOut(context.HttpContext);
                return Task.CompletedTask;
            }
        }
    }
}
