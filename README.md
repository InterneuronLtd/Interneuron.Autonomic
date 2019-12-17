# Interneuron Autonomic

## Introduction

This Read Me is provided to give guidance on the installation and configuration of Interneuron Autonomic


## Prerequisites 

**Other Synapse Project Dependencies**
```
1. Synapse Dynamic API
2. Synapse Identity server
3. Synapse Dynamic API Client
```
**Development**

```
1. Visual Studio Community / Visual Studio Code
2. ASP.NET Core SDK 2.1 or higher 
```
**Deployment** [windows]

```
1. IIS 7 or higher
```
## Installation & Configuration

This section guides you through the deployment and configuration of Interneuron Autonomic web API.

### Interneuron Autonomic web API

 Configure appsetting [Interneuron.Autonomic/Interneuron.Autonomic/appsetting.josn](Interneuron.Autonomic\Interneuron.Autonomic/appsetting.json)

​		a. update Authorization / Authority setting with Synapse Identity Server URL

​		b. Update Synapse Dynamic API URL

```json
"Settings": {  
    "AuthorizationAuthority": "YOUR_SYNAPSE_IDENTITY_SERVER_URL",
   }
"DynamicAPISettings": {
    "uri": "YOUR_DYNAMIC_API_URL"
   
  }
```

#### Create the package SynapseDynamicAPIClient

1. URL:- [/Interneuron.Synapse/SynapseDynamicAPIClient]()

2. In command line or PowerShell, navigate to your project directory.

3. Run: dotnet pack  If all goes well you should now have a generated .nupkg file 

4. Copy your created Package to local or network path

5. In Visual Studio go to the Tools menu > Options.
	*  Open the Nuget Package Manager node
	* Add a new Repository to a local or network path
	* Ensure the new repository is ticked active.

   

### Publish and Install

```
Use visual studio to publish the application directly to a web service or,
```

1. Publish to a folder profile

2. Locate and copy the Interneuron-AppPools.xml and Interneuron-Sites.xml in Sample/IISSettings folder

3. Open command prompt in administrator mode and execute the below commands

4. Copy the published code into the websites physical path. 

5. Open command prompt and run IISRESET


```
   %windir%\system32\inetsrv\appcmd add apppool /in < "path to Interneuron-AppPools.xml"
   
   %windir%\system32\inetsrv\appcmd add site /in < "path to  Interneuron-Sites.xml"
```

6. .Browse to http://your-chosen-url/SynapseStudio/RegisterAsSuperUser.aspx and provide credentials to register your super user account.
7. Use the new super register to logon.




## Author

* GitHub: [Interneuron CIC](https://github.com/InterneuronCIC)
## License
Interneuron Synapse

Copyright(C) 2019  Interneuron CIC 

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. 

If not, see <http://www.gnu.org/licenses/>.