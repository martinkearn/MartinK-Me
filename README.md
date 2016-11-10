# Technical Evangelist Site
A template website written in ASP.net Core 1.0 that can be used to create a personal website for technical evangelists that can be used to promote articles, links, resources and talks. 

My own personal example is published at [MartinK.me](http://MartinK.me)

## Customisation
This site is setup to be customisation by you. If you want to do this, first fork it to your own account and then start modifying your version.

Here are some of the things you should consider doing after you have your fork:
* Update `AppSettings.json` with your details and all the personal stuff will be updated throughout the site
* Go to `/admin` for the administration pages to add data
* Make sure you use your own database. 
  * Create an Azure SQL database and grab the ADO.net connection string
  * Add the connection string (with your username password in it) as the value of the `DefaultConnection` in `AppSettings.json`
  * Use the entity framework `add-migration Initial` and `update-database` from the Package Manager Console to add the model schema to your database
* Experiment with your own fonts, colours, picture and copy - make it your own
  * The site is built using [Bootstrap](http://getbootstrap.com/) so familiarise yourself with the [Bootrap Grid System](http://getbootstrap.com/css/#grid) at the very least.
