using JWTApi.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace JWTApi
{
    public static class StartupExtension
    {

        public static IServiceCollection ConfigurarJWKToken(this IServiceCollection services, IConfiguration configuration, SigningConfigurations signingConfigurations)
        {
            var configuracoesToken = new ConfiguracoesToken();
            new ConfigureFromConfigurationOptions<ConfiguracoesToken>(
                configuration.GetSection("TokenConfigurations"))
                    .Configure(configuracoesToken);

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddSingleton(configuracoesToken);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions =>
            {

                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = configuracoesToken.Audience;
                paramsValidation.ValidIssuer = configuracoesToken.Issuer;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;


                // Ativa o uso do token como forma de autorizar o acesso
                // a recursos deste projeto
               
            });

            return services;
        }

    }
}
