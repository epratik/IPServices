# IPServices
aggregate results from multiple ip services  
Working -  
1.Input services are sent on different kafka topics based on name and message value as IP address.  
2.There are separate worker service applications for each service provided.  
3.These workers are consumers to the specific kafka topics. When a message is available, it is processed and written to redis hash set with IP address as the key.  
4.The web api polls the redis cache for output of the selected services and returns the results.  
5.Any subsequent requests for same service and same IP are server from the cache.  
6.If 3 services are requested and 2 are present in cache. API would only send 1 request to kafka.  
7.All the worker API's are multithreaded and can by default work on 3 messages from kafka.  

App Settings File -  IPServiceAggregator
"LogPath": "SPECIFY A LOG PATH"  
"defaultServices": "rdap,geoip,ping"  
"RedisConn": "localhost:6379"  
"KafkaConn": "localhost:9092"  
"RetryCount": "SPECIFY RETRY COUNT FOR POLLING RESULTS OF VARIOUS SERVICES"    

App Settings File - RDAP Worker Service  
"KafkaConn": "localhost:9092"  
"RedisConn": "localhost:6379"  
"rdap": "https://rdap.apnic.net/ip/{0}"  
"MaxThreads":"SPECIFY MAX THREADS FOR THE WORKER"

App Settings File - GEOIP Worker Service  
"KafkaConn": "localhost:9092"  
"RedisConn": "localhost:6379"  
"geoip": "https://ipapi.co/{0}/json/"  
"MaxThreads":"SPECIFY MAX THREADS FOR THE WORKER"  
 
App Settings File - PING Worker Service  
"KafkaConn": "localhost:9092"  
"RedisConn": "localhost:6379"  
"MaxThreads":"SPECIFY MAX THREADS FOR THE WORKER"  

Rate Limit  
Rate Limit is set to 10 seconds can be changed in controller attribute.  

Services Supported - geoip, rdap and ping

You can access swagger URL on run - https://localhost:{PORT}/swagger/index.html  

Sample Get Request - https://localhost:44306/api/IP/104.18.130.189/services?Services=geoip    

Sample request for default services - https://localhost:44306/api/IP/104.18.130.189/services  

To Test -  
1.Setup Redis  
2.Setup Kafka  
3.Run the application. Multiple projects will start.  
4.Open swagger and test the input.  

Future enhancements -  
1.Convert Service output json to a internal DTO. This will provide better unit testing.  
3.Better exception handling.  
4.Add IP or domain support in the input. This version handles only IP input.  





