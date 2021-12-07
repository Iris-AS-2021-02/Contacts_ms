# Contacts_Ms

## Database Docker Commands
- docker pull mcr.microsoft.com/mssql/server
- docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Contacts.2021" -p 1433:1433 --name iris_contacts_db -h iris_contacts_db -d mcr.microsoft.com/mssql/server

## MicroService Docker Commands
- docker build -t iris_contacts_ms .
- docker run -d --name iris_contacts_ms -p 8088:88 iris_contacts_ms

## Endpoints

- GET: /Contacts/GetContacts?userId=61a415a70c8dbe6a59316e93

- GET: /Contacts/GetContact?contactId=a088423f-2d0c-4def-ca1e-08d9b907fa8c

- POST: /Contacts/Synchronize?userId=61a415a70c8dbe6a59316e93
```json
[
    {
        "contactPhone": "+573057174334",
        "contactName": "Harold Bartolo"
    }
]
```

- POST: /Contacts/ChangeOptions
```json
{
  "contactID": "a088423f-2d0c-4def-ca1e-08d9b907fa8c",
  "blocked": false,
  "seeStatus": true,
  "URIwallpaper": "string with format => data:[<mediatype>][;base64],<data> => data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEASABIAAD/",
  "extension" : "jpeg",
  "removeCurrentWallpaper": false
}
```
