# CrmSessionCookieRenamer
Automatically rename the cookie of dynamics 365 web site.



Passive federation request fails when accessing an application using AD FS and Forms Authentication after previously connecting to Microsoft Dynamics CRM also using AD FS.


use ADFS 3.0 as IDP for dynamics 365 and others, 
if access dynamics 365 first, then redirect to other sso site, it can not login automatically. 
the authentication window will prompt .

problem ref 1:   
https://community.dynamics.com/crm/f/117/t/190404

problem ref 2:    
https://crmtipoftheday.com/546/avoid-using-the-same-domain-for-adfs-and-crm/


there is a deployment way to reslove this problem.     
ref : https://support.microsoft.com/en-us/help/3045286/passive-federation-request-fails-when-accessing-an-application-using-a


 there is a simple way to resolve this problem by rename the cookie name. 

the dynamics cookie writed by HttpModule: CrmSessionAuthenticationManager.
This module use ICookieHandler to write the cookie to browser. 
We have a chance to rename the cookie name before the cookie be writed to browser.


IIS support dynamic module, we can dynamicly register a new HttpModule,
When this module initing, modify another module which initialized before .
Here is the code.


``` csharp

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

```


ref the Nuget package :  Microsoft.Web.Infrastructure

```csharp

public static class CrmAuthenCookieRenameHelper
{
	public static void Start()
	{
		// The dynamic module will be add after the modules which defined in web.config file.
		DynamicModuleUtility.RegisterModule(typeof(CrmAuthenCookieRenameModule));
	}
}

```

and register it for auto start when site restart.

```csharp

[assembly: PreApplicationStartMethod(typeof(CrmAuthenCookieRenameHelper), "Start")]

```


build the project. and copy two dlls to CRMWeb\bin, it work fine.

<b>
warning: This is an unsupported approach, full testing before apply it to production environments.
	</b>



