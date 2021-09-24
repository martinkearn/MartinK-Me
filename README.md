# MartinK.me Site
[![Pulumi](https://github.com/martinkearn/MartinK-Me/actions/workflows/push.yml/badge.svg)](https://github.com/martinkearn/MartinK-Me/actions/workflows/push.yml)

A website written in ASP.net Core 1.0 that can be used to create a personal website for technical evangelists which can be used to promote articles, links, resources and talks. 

My own personal site is published at [MartinK.me](http://MartinK.me) which is published from this code base.

## Customisation
This site is setup to be customised. If you want to do this, first fork it to your own account and then start modifying your version. Out of courtesy, please try to implement your own visual style and do not re-use the fonts and colours I've use don my own personal site.

Here are some of the things you should consider doing after you have your fork:
* Update `AppSettings.json` with your details and all the personal stuff will be updated throughout the site
* Go to `/admin` for the administration pages to add data
* Make sure you use your own cloud database. 
  * Create an Azure SQL database and grab the ADO.net connection string
  * Add the connection string (with your username password in it) as the value of the `DefaultConnection` in `AppSettings.json`
  * Use the commands `dotnet ef migrations add YourMigrationNameHere` and `dotnet ef database update` from the console (naviagted to the project root directory, the same directory as `EvangelistSiteWeb.xproj`) to add the model schema and update your database
* Experiment with your own fonts, colours, picture and copy; make it your own
  * The site is built using [Bootstrap](http://getbootstrap.com/) so familiarise yourself with the [Bootrap Grid System](http://getbootstrap.com/css/#grid)
  * The background colours are in a range of CSS styles entitled `.sequence-background-1` through to `.sequence-background-20` in `site.css`. In the template these are four base colours repeated 5 times, but you can add your own colours here and have 20 different colours if you want.
  * The font is Lato from Google Fonts. Choose your own font at https://fonts.google.com and edit the `body` rule in site.css to reference it

## Settings
There is a 'settings' section under `/admin`. This is used to simply store key/value pairs which can be used in any way you set fit for other applications and demo that you have running but need a simple storage layer for. There is an API under `/api/settings` which you can use to `GET` all settings or individual settings by id, for example `/api/settings/5`. 

The settings have no bearing on this application, but can be usefull in other applications.

To change the settings and data for this application, see `AppSettings.json`.
