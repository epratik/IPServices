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



