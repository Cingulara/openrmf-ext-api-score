# OpenRMF-ext-api-score
This is the OpenRMF Score API that is used for integration with external applications. THis is a proof of concept using Kong API Gateway calling a service within OpenRMF that performs a function. You must setup Kong API Gateway for this, and include the ACL and Key-Auth plugins for it to work correctly for security sake.

## API Calls
POST to / with the field rawChecklist having the raw XML of a checklist gives a score with your data if it is a valid checklist file.

GET to /swagger/ gives you the API structure.

## Making your local Docker image
docker build --rm -t openrmf-ext-api-score:0.13.01 .

## Linking to Kong API Gateway

If Kong API GW is loaded (see https://github.com/Cingulara/openrmf-docs for more information on running locally) you can setup a backend service to point to this API. And then setup a route with plug-ins for external applications to call it. 

For this Kong API Gateway I am going to use the command line and the Kong API Gateway OSS version. If you use the Enterprise version this is _A LOT_ easier. Or you can use one of the free GUIs like Konga https://github.com/pantsel/konga/blob/master/README.md#compatibility or https://github.com/pocketdigi/kong-admin-ui. 

### Setup the Service (backend) using the Kong Admin API
```
curl -i -X POST \
  --url http://localhost:8001/services/ \
  --data 'name=openrmf-ext-api-score' \
  --data 'url=http://192.168.13.23:8100'
```

You will receive something like this with the ID, protocol, timeout, etc:

```
HTTP/1.1 201 Created
Date: Sun, 29 Mar 2020 22:10:23 GMT
Content-Type: application/json; charset=utf-8
Connection: keep-alive
Access-Control-Allow-Origin: *
Server: kong/2.0.2
Content-Length: 300
X-Kong-Admin-Latency: 17

{"host":"192.168.13.23","created_at":1585519941,"connect_timeout":60000,"id":"b241c2d8-5c01-42da-9648-d600df81ef60","protocol":"http","name":"openrmf-ext-api-score","read_timeout":60000,"port":8100,"path":null,"updated_at":1585519941,"retries":5,"write_timeout":60000,"tags":null,"client_certificate":null}
```

### Setup the Route (frontend you call) using the Kong Admin API

```
curl -i -X POST \
  --url http://localhost:8001/services/openrmf-ext-api-score/routes \
  --data 'name=openrmf-ext-api-score-route' \
  --data 'methods=POST' \
  --data 'paths[]=/scorechecklist'
```

You will receive a response like below with relevant information:

```
HTTP/1.1 201 Created
Date: Sun, 29 Mar 2020 22:22:53 GMT
Content-Type: application/json; charset=utf-8
Connection: keep-alive
Access-Control-Allow-Origin: *
Server: kong/2.0.2
Content-Length: 463
X-Kong-Admin-Latency: 13

{"id":"7855c546-8b2b-4ab4-8df3-26369f888c3d","path_handling":"v0","paths":["\/scorechecklist"],"destinations":null,"headers":null,"protocols":["http","https"],"methods":["POST"],"snis":null,"service":{"id":"b241c2d8-5c01-42da-9648-d600df81ef60"},"name":"openrmf-ext-api-score-route","strip_path":true,"preserve_host":false,"regex_priority":0,"updated_at":1585520573,"sources":null,"hosts":null,"https_redirect_status_code":426,"tags":null,"created_at":1585520573}
```

### Enable ACL and Key-Auth Plugins on the Route
Run the below command with the name or ID of the route you created. This lets you put the key-auth plugin on it. 

```
curl -X POST http://localhost:8001/routes/openrmf-ext-api-score-route/plugins \
    --data "name=key-auth" \
    --data "config.key_names=openrmf-ext-key" \
    --data "config.hide_credentials=true"
```

You should receive something back like the below showing it was created.
```
{"created_at":1585521291,"config":{"key_names":["openrmf-ext-key"],"run_on_preflight":true,"anonymous":null,"hide_credentials":true,"key_in_body":false},"id":"aea7c6a0-c508-4ed2-a4e1-0fa14c8615b0","service":null,"enabled":true,"protocols":["grpc","grpcs","http","https"],"name":"key-auth","consumer":null,"route":{"id":"7855c546-8b2b-4ab4-8df3-26369f888c3d"},"tags":null}
```

Now you need to make a *Consumer* to call this API with the proper key-auth and key name. You could even use the GUID Generator at https://www.guidgenerator.com/online-guid-generator.aspx for a custom_id. As long as you get a positive response back you are good to go to use this. 

```
curl -d "username=cingulara&custom_id=8ab7a947-7e57-4b52-b4df-f0c5c520870f" http://localhost:8001/consumers/
```

Finally give them a *Key) using Kong by running the following command, replacing "cingulara" with the consumer name above:
```
curl -X POST http://localhost:8001/consumers/cingulara/key-auth -d ''
```
You are going to get something like the below. It has the key in it for use. 
```
{"created_at":1585521556,"consumer":{"id":"50b8ad1b-791a-46c8-a84a-be305e815d61"},"id":"20a42281-cf0e-4ead-a2e3-41b85884551e","tags":null,"ttl":null,"key":"pQywVhHMLAc6maccMTyN7zuG43XrbJ6C"}
```

Now if you call that same POST /scorechecklist/ with the XML data payload you will receive a message back like below. 
```
{
  "message": "No API key found in request"
}
```

You will need to add the header "openrmf-ext-key" for the config name we added above and put in the "key" from the key-auth post just above. That will match a valid key to the request and allow the checklist score to be returned successfully. An example using Insomnia is below. 

![Image](./img/Calling-Kong-API-Key-Auth.png?raw=true)

To enable the ACL Plugin you would do the following which allows the group to use it:
```
curl -X POST http://localhost:8001/routes/openrmf-ext-api-score-route/plugins \
    --data "name=acl"  \
    --data "config.whitelist=openrmf-ext-score" \
    --data "config.hide_groups_header=true"
```

Then you need to add the consumer we made above to the group, otherwise you get the "you cannot consume this service" message:
```
curl -X POST http://localhost:8001/consumers/cingulara/acls \
    --data "group=openrmf-ext-score"
```

### Calling the API behind Kong API Gateway

You would need to call `http://localhost:8000/scorechecklist` with a POST and pass in the rawChecklist field with the raw data in the CKL file to get this API to work. 


## More Information
See https://docs.konghq.com/2.0.x/getting-started/configuring-a-service/.

Services: https://docs.konghq.com/2.0.x/admin-api/#add-service

Routes: https://docs.konghq.com/2.0.x/admin-api/#add-route

Rate Limiting: https://docs.konghq.com/hub/kong-inc/rate-limiting/ could also be included
