Simple Site Monitor
========================

Simple console application that makes web requests to the configured URLs, validating that they work correctly. Run on a schedule (e.g., Windows scheduled task) for a simple website monitoring solution. 

Background
---------------------
I wanted to monitor the availability of various website that I developer and/or host. Searching the Internet yielded a lot of nice pay options and some limited functionality free options. I figured I'd create my own basic monitor and just run it as a scheduled task on my Windows desktop at home. Been running it for years now and it's worked perfectly, giving me the simple monitoring that I needed. 

Features
---------------------

* **Website Monitor**
	* Configure as many URLs to monitor as you like
	* Checks for an HTTP status 200 response
	* Also checks for the existence of expected text in the HTML returned in the response
* **Email Notifications**
	* Configure your SMTP credentials and send email alerts when a URL monitor fails
	* Includes details on the failure, i.e., HTTP status code, if expected text wasn't found, etc.

How to Use
---------------------	

1. Build the solution
2. Take a copy of *SimpleSiteMonitor.exe* and *SimpleSiteMonitor.exe.config* from the build output and place them in a location from where you want the application to run
3. Open *SimpleSiteMonitor.exe.config* in a text editor
4. Configure the URLs to monitor:
	- Create entries in the *appSettings* node:
	```xml
	&lt;appSettings&gt;
		&lt;!--add key="uniqueName" value="http://yourUrlHere.com,text to look for in HTML"/--&gt;
		&lt;add key="google" value="http://www.google.com,I'm feeling lucky"/&gt;
	&lt;/appSettings&gt;
	```
	- Set the SMTP values for sending email:
	```xml
	&lt;applicationSettings&gt;
		&lt;SimpleSiteMonitor.Settings&gt;
			&lt;setting name="SmtpServer" serializeAs="String"&gt;
				&lt;value&gt;smtp.gmail.com&lt;/value&gt;
			&lt;/setting&gt;
			&lt;setting name="SmtpPort" serializeAs="String"&gt;
				&lt;value&gt;587&lt;/value&gt;
			&lt;/setting&gt;
			&lt;setting name="FromAddress" serializeAs="String"&gt;
				&lt;value&gt;youraccount@gmail.com&lt;/value&gt;
			&lt;/setting&gt;
			&lt;setting name="ToAddress" serializeAs="String"&gt;
				&lt;value&gt;youraccount@gmail.com&lt;/value&gt;
			&lt;/setting&gt;
			&lt;setting name="Password" serializeAs="String"&gt;
				&lt;value&gt;yourPassword&lt;/value&gt;
			&lt;/setting&gt;
		&lt;/SimpleSiteMonitor.Settings&gt;
	&lt;/applicationSettings&gt;
	```
5. Create a Windows scheduled task to execute *SimpleSiteMonitor.exe" on the schedule of your choosing
6. Done