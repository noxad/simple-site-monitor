Simple Site Monitor
========================

Simple console application that makes web requests to the configured URLs, validating that they work correctly. Run on a schedule (e.g., Windows scheduled task) for a simple website monitoring solution. 

Background
---------------------
I wanted to monitor the availability of various website that I developer and/or host. Searching the Internet yielded a lot of nice pay options and some limited functionality free options. I figured I'd create my own basic monitor and just run it as a scheduled task on my Windows desktop at home. Been running it for years now and it's worked perfectly, giving me the simple monitoring that I needed. 

Search for expected text uses regular expression matching because it was quick and simple. I acknowledge that using regular expressions to parse HTML is generally not advisable, but due to the simplicity of the implementation here, it works just fine. Read up [here](http://stackoverflow.com/questions/1732348/regex-match-open-tags-except-xhtml-self-contained-tags) on why you shouldn't use regular expressions to parse HTML, using an XML parser instead. 

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
	```
	<!--add key="uniqueName" value="http://yourUrlHere.com,text to look for in HTML"/-->
	```
	```
	<add key="google" value="http://www.google.com,I'm feeling lucky"/>
	```
	- Set the SMTP values for sending email in the *applicationSettings* node:
	```
	<setting name="SmtpServer" serializeAs="String">
	```
	```
		<value>smtp.gmail.com</value>
	```
	```
	</setting>
	```
	```
	<setting name="SmtpPort" serializeAs="String">
	```
	```
		<value>587</value>
	```
	```
	</setting>
	```
	```
	<setting name="FromAddress" serializeAs="String">
	```
	```
		<value>youraccount@gmail.com</value>
	```
	```
	</setting>
	```
	```
	<setting name="ToAddress" serializeAs="String">
	```
	```
		<value>youraccount@gmail.com</value>
	```
	```
	</setting>
	```
	```
	<setting name="Password" serializeAs="String">
	```
	```
		<value>yourPassword</value>
	```
	```
	</setting>
	```
5. Create a Windows scheduled task to execute *SimpleSiteMonitor.exe" on the schedule of your choosing
6. Done