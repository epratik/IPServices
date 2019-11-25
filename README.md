# IPServices
aggregate results from multiple ip services

App Settings File -  
"LogPath": "SPECIFY A LOG PATH"  
"defaultServices": "rdap,geoip,ping"  
"geoip": "https://ipapi.co/{0}/json/"   
"rdap": "https://rdap.apnic.net/ip/{0}"  

Rate Limit  
Rate Limit is set to 10 seconds can be changed in controller attribute.  

Services Supported - geoip, rdap and ping

You can access swagger URL on run - https://localhost:{PORT}/swagger/index.html  

Sample Get Request - https://localhost:44306/api/IP/104.18.130.189/services?Services=geoip

Sample request for default services - https://localhost:44306/api/IP/104.18.130.189/services

Future enhancements -  
1.Convert Service output json to a internal DTO. This will provide better unit testing.
2.Removing the Logging dependency.
3.Better exception handling. Right now there is only a central filter and per service exception handler for partial results.  





