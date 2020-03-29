# OpenRMF-ext-api-score
This is the OpenRMF Score API that is used for integration with external applications. 

## API Calls
POST to / with the field rawChecklist having the raw XML of a checklist gives a score with your data if it is a valid checklist file.

GET to /swagger/ gives you the API structure.

## Making your local Docker image
docker build --rm -t openrmf-ext-api-score:0.13.01 .