# Contacts_Ms

## Database Docker Commands
- docker pull mcr.microsoft.com/mssql/server
- docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Contacts.2021" -p 1433:1433 --name iris_contacts_db -h iris_contacts_db -d mcr.microsoft.com/mssql/server

## MicroService Docker Commands
- docker build -t iris_contacts_ms .
- docker run --name iris_contacts_ms -p 8088:88 iris_contacts_ms

## Endpoints

- GET: /Contacts/GetContacts?userId=1

- GET: /Contacts/GetContact?contactId=1

- POST: /Contacts/Synchronize?userId=1
```json
[
    {
        "contactPhone": "string",
        "contactName": "string"
    }
]
```

- POST: /Contacts/ChangeOptions
```json
{
  "contactID": 1,
  "blocked": false,
  "seeStatus": true,
  "URIwallpaper": "string with format => data:[<mediatype>][;base64],<data> => data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEASABIAAD/",
  "extension" : "jpeg",
  "removeCurrentWallpaper": false
}
```