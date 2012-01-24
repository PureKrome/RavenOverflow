Start typing in the blue box...# RavenOverflow

![PewPew!](http://static.ravendb.net/badges/standard/badge1.png)

### AIM OF THIS PROJECT

To help people getting started with RavenDb by showing some common problems and how to solve those problems using RavenDb as the database/persistence.

### Introduction

This is a sample ASP.NET MVC3 application that leverages [RavenDB](Http://RavenDB.net) as the database / persistence store.
I've copied [StackOverflow](http://stackoverflow)'s home page, include css and images. It looks a lot like it ... but I'm not a CSS/HTML Gandalf so it's a bit rough around the edges.
Because I'm putting my emphasis on the how to use RavenDB .. I'm not putting much energy into the UI. If it looks similar/OK to StackOverflow, then that's enough :)

Like [Racoon Blog](https://github.com/ayende/RaccoonBlog) (another sample application which is actually being used in production scenarios), this project is trying to show the benefits of RavenDb in an ASP.NET MVC3 framework.

### What it Does

* List some fake questions. Summarize answers / views / tags.
* Shows how to do simple queries and leverage Indexes.
* Shows how to batch page queries using RavenDb

### What it Uses

* [RavenDb](http://ravendb.net/) v606-Unstable / RavenDb Embedded v606-Unstable
* [ASP.NET MVC3](http://asp.net/mvc/)
* [StructureMap](http://structuremap.net/structuremap/) for IoC
* [NBuilder](http://nbuilder.org/) for fake data
* [xUnit](http://xunit.codeplex.com/) for Tests
* An attempt to simulate a production solution layout (eg. n-Tier, IoC, InputModels/ViewModels)

### What it is NOT

* A best practice application. I'm a noob, so this is a learning experience.

### Support / Help

* I'm usually idling on [JabbR RavenDB room](http://jabbr.net/#/rooms/RavenDB). Please drop by! :)
* I accept Pull Requests, btw (hint hint) :)