Gac4Net  
======================

[![Build status](https://ci.appveyor.com/api/projects/status/l7oc0197mgghmx13?svg=true)](https://ci.appveyor.com/project/igorkulman/appstorecrawler)

Gac4Net is short of google app crawl for net.Simple scalable crawler for Apps data from the [Play Store](https://play.google.com/store).

You don't have to input any of your Google Account credentials since this Crawler acts like a "Logged Out" user.

# Exporting the Database
**For the database** this project used is mongodb(version 2.6 recommended,3.0 will be also supported),you can just install mongodb,and create the datebase

* PlayStore, collections:ProcessedApps,QueuedApps
* GOOGStore, collections:UserInfo

you do not need to create the table(the program will do it for you).

#what data crawled?
Gac4Net crawl the below information

* (1)The **app detail information** from [Google app store](https://play.google.com/store)
* (2)**[Google+](https://plus.google.com/) User information** who use google app,and the user app use history
* (3)The user's **Reviewers**

these all information Guaranteed the info wholeness,these information will sufficient for research.In my machine,Gac4Net have crawled about 40M apps information ,20M [Google + ](https://plus.google.com/),and 100M user Reviews. 

Even though the program down,you can start it again ,then it will work again.

# About me
My name is swinghu, i am a  developer from China who study at WHU at the moment, Knowing C/C++,Java,C#,Liking backend developing.

Email: ogrecpp@gmail.com

Personal page:[http://swinghu.github.com](http://swinghu.github.com)

# What is this project about ? 

The main idea of this project is to gather/mine data about apps of the Google Play Store and build a rich database so that developers, android fans and anyone else can use to generate statistics about the current play store situation

There are many questions we have no answer at the moment and we should be able to answer than with this database.

# What do i need before i start?

* Recommend you read all the pages of this wiki, which won`t take long.
* Before ask,Read the code first ,Do Second!
* Know C#, mongodb and visual studio.

# The future work
Because this project only crawl the app from [google play](https://play.google.com/store),it not include any other app store like apple,this is Insufficient.
So the next step,Gac4Net will embrace [apple store](http://store.apple.com/us) and any other local store(especial the store in China like [安卓市场](http://apk.hiapk.com/) etc.).Suggestion is most welcome to me.

**Refer to the Pages section of this wiki for individual information about each aspect of the project.**
