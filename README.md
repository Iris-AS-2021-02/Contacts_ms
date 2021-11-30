# Contacts_Ms

### Database Docker Commands
- docker pull mcr.microsoft.com/mssql/server
- docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Contacts.2021" -p 1433:1433 --name iris_contacts_db -h iris_contacts_db -d mcr.microsoft.com/mssql/server
- 
### MicroService Docker Commands
- docker build -t iris_contacts_ms .
- docker run --name iris_contacts_ms -p 8080:80 iris_contacts_ms
