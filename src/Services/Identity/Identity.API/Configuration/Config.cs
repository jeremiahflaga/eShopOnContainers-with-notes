using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Configuration
{
    public class Config
    {
        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("orders", "Orders Service"),
                new ApiResource("basket", "Basket Service"),
                new ApiResource("mobileshoppingagg", "Mobile Shopping Aggregator"),
                new ApiResource("webshoppingagg", "Web Shopping Aggregator"),
                new ApiResource("orders.signalrhub", "Ordering Signalr Hub"),
                new ApiResource("webhooks", "Webhooks registration Service"),
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "eShop SPA OpenId Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    // NOTE_JBOY: to allow to pass tokens via browser URL (from "ASP.NET Core, C#, IdentityServer4 - Authentication - Tricking Library Ep28" - https://www.youtube.com/watch?v=Ql0ZB67J0TQ)
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris =           { $"{clientsUrl["Spa"]}/" },
                    // NOTE_JBOY: Consent screen is only needed if there is a thrid party client app that wants to authenticate with us (from "ASP.NET Core, C#, IdentityServer4 - Authentication - Tricking Library Ep28" - https://www.youtube.com/watch?v=Ql0ZB67J0TQ)
                    // If you are the one creating the client and you know all the information you will grab, you will NOT need a consent screen.
                    RequireConsent = false,
                    PostLogoutRedirectUris = { $"{clientsUrl["Spa"]}/" },
                    // NOTE_JBOY: Because people are going from different domains to authenticate, you need to specify CORS origins.
                    // We are basically saying "we are expecting authentication from this guy, and then this is something we can validate " (from "ASP.NET Core, C#, IdentityServer4 - Authentication - Tricking Library Ep28" - https://www.youtube.com/watch?v=Ql0ZB67J0TQ)
                    AllowedCorsOrigins =     { $"{clientsUrl["Spa"]}" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "orders",
                        "basket",
                        "webshoppingagg",
                        "orders.signalrhub",
                        "webhooks"
                    },
                },
                new Client
                {
                    ClientId = "xamarin",
                    ClientName = "eShop Xamarin OpenId Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,                    
                    //Used to retrieve the access token on the back channel.
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = { clientsUrl["Xamarin"] },
                    RequireConsent = false,
                    // NOTE_JBOY: to allow code flow through the browser (from "ASP.NET Core, C#, IdentityServer4 - Authentication - Tricking Library Ep28" - https://www.youtube.com/watch?v=Ql0ZB67J0TQ)
                    RequirePkce = true,
                    PostLogoutRedirectUris = { $"{clientsUrl["Xamarin"]}/Account/Redirecting" },
                    //AllowedCorsOrigins = { "http://eshopxamarin" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "orders",
                        "basket",
                        "mobileshoppingagg",
                        "webhooks"
                    },
                    //Allow requesting refresh tokens for long lived API access
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    ClientUri = $"{clientsUrl["Mvc"]}",                             // public uri of the client
                    // NOTE_JBOY: how our application is going to authenticate -- the code flow (from "ASP.NET Core, C#, IdentityServer4 - Authentication - Tricking Library Ep28" - https://www.youtube.com/watch?v=Ql0ZB67J0TQ)
                    AllowedGrantTypes = GrantTypes.Hybrid, 
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    // NOTE_JBOY: enable support for refresh tokens via the AllowOfflineAccess property (from https://docs.identityserver.io/en/latest/quickstarts/3_aspnetcore_and_apis.html)
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    // NOTE_JBOY: where to go to authenticate (from "ASP.NET Core, C#, IdentityServer4 - Authentication - Tricking Library Ep28" - https://www.youtube.com/watch?v=Ql0ZB67J0TQ)
                    RedirectUris = new List<string> 
                    {
                        $"{clientsUrl["Mvc"]}/signin-oidc"
                    },
                    // NOTE_JBOY: where to go after logout(from "ASP.NET Core, C#, IdentityServer4 - Authentication - Tricking Library Ep28" - https://www.youtube.com/watch?v=Ql0ZB67J0TQ)
                    PostLogoutRedirectUris = new List<string> 
                    {
                        $"{clientsUrl["Mvc"]}/signout-callback-oidc"
                    },
                    // NOTE_JBOY: Scope is the area of information or category of information you want to extract. (from "ASP.NET Core, C#, IdentityServer4 - Authentication - Tricking Library Ep28" - https://www.youtube.com/watch?v=Ql0ZB67J0TQ)
                    AllowedScopes = new List<string> 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "orders",
                        "basket",
                        "webshoppingagg",
                        "orders.signalrhub",
                        "webhooks"
                    },
                    AccessTokenLifetime = 60*60*2, // 2 hours
                    IdentityTokenLifetime= 60*60*2 // 2 hours
                },
                new Client
                {
                    ClientId = "webhooksclient",
                    ClientName = "Webhooks Client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    ClientUri = $"{clientsUrl["WebhooksWeb"]}",                             // public uri of the client
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = new List<string>
                    {
                        $"{clientsUrl["WebhooksWeb"]}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{clientsUrl["WebhooksWeb"]}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "webhooks"
                    },
                    AccessTokenLifetime = 60*60*2, // 2 hours
                    IdentityTokenLifetime= 60*60*2 // 2 hours
                },
                new Client
                {
                    ClientId = "mvctest",
                    ClientName = "MVC Client Test",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    ClientUri = $"{clientsUrl["Mvc"]}",                             // public uri of the client
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    RedirectUris = new List<string>
                    {
                        $"{clientsUrl["Mvc"]}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{clientsUrl["Mvc"]}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "orders",
                        "basket",
                        "webshoppingagg",
                        "webhooks"
                    },
                },
                new Client
                {
                    ClientId = "basketswaggerui",
                    ClientName = "Basket Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["BasketApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["BasketApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "basket"
                    }
                },
                new Client
                {
                    ClientId = "orderingswaggerui",
                    ClientName = "Ordering Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["OrderingApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["OrderingApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "orders"
                    }
                },
                new Client
                {
                    ClientId = "mobileshoppingaggswaggerui",
                    ClientName = "Mobile Shopping Aggregattor Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["MobileShoppingAgg"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["MobileShoppingAgg"]}/swagger/" },

                    AllowedScopes =
                    {
                        "mobileshoppingagg"
                    }
                },
                new Client
                {
                    ClientId = "webshoppingaggswaggerui",
                    ClientName = "Web Shopping Aggregattor Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["WebShoppingAgg"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["WebShoppingAgg"]}/swagger/" },

                    AllowedScopes =
                    {
                        "webshoppingagg",
                        "basket"
                    }
                },
                new Client
                {
                    ClientId = "webhooksswaggerui",
                    ClientName = "WebHooks Service Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["WebhooksApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["WebhooksApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "webhooks"
                    }
                }
            };
        }
    }
}