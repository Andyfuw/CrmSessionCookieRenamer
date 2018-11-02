using System.IdentityModel.Services;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using RekTec.Crm.SessionCookieUnsafeRenamer;

[assembly: PreApplicationStartMethod(typeof(CrmAuthenCookieRenameHelper), "Start")]

namespace RekTec.Crm.SessionCookieUnsafeRenamer
{
	public static class CrmAuthenCookieRenameHelper
	{
		public static void Start()
		{
			// The dynamic module will be add after the modules which defined in web.config file.
			DynamicModuleUtility.RegisterModule(typeof(CrmAuthenCookieRenameModule));
		}
	}


	// This module is do nothing but rename the cookieName, 
	public class CrmAuthenCookieRenameModule : IHttpModule
	{
		// This is the new cookie name which will be send to browser.
		private static readonly string CrmCookieName = "CRM_MSISAuth";

		public void Init(HttpApplication context)
		{
			var sessionAuthModule = context.Modules["CrmSessionAuthenticationManager"] as SessionAuthenticationModule;

			if (sessionAuthModule != null)
			{
				var cookieHandler = sessionAuthModule.CookieHandler;
				if (cookieHandler != null)
				{
					if(!CrmCookieName.Equals(cookieHandler.Name))
					{
						cookieHandler.Name = CrmCookieName;						
					}

					// we can change domain here, but it will be more complex to get the domain name.
					// we should sure it can be work in intranet & extranet ...
					// cookieHandler.Domain
				}
			}
		}


		public void Dispose()
		{
			//nothing to do here
		}

	}
}
