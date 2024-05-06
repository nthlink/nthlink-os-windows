# nthlink Windows App

![Build Status](https://github.com/teoncom/nthLink-windows/actions/workflows/Release.yml/badge.svg?event=push)

## Introductions

[nthlink](https://www.nthlink.com/) is an anti-censorship mobile application capable of
circumventing Internet censorship and self-recovering from blocking events.

The purpose of nthlink is enabling everyday users in censored regions to gain safer and unfettered
access to the Internet.

## UX / UI

Basically, our app has only one main function, the other functions are related to customer service.

### Home Page

![Home page](Readme/HomePage.png)

There is one button named connect in home page, which is the function of start the connection.
After connect to VPN successfully, it launch a news website on default browser at private mode automatically,
and home page expand right view too.
Click the disconnect button when connected, nthlink will disconnect to VPN server.

#### Home page right view

![home page right view](Readme/HomePageExpand.png)

Here are some announcement and hot news, users can click to open websites on default browser at private mode.

### Home Page Menu

There is menu button at top left in home page will pop menu when click. 
Menu has three button items feedback, about and help.
Feedback and about button will navigate to their page when click, and help button open [help website](https://s3.us-west-1.amazonaws.com/dwo-jar-kmf-883/help.html) on default browser at private mode.

### Toast Window

![toast window](Readme/ToastWindow.png)

On the top-right corner of Home page is notification toggle button.
We will push hot news by toast window when toggle was on, The toast window show at bottom right on Windows OS.

### Feedback Page

![feedback page](Readme/FeedbackPage.png)

Users can fill up the form and feedback to us.

### About Page

![about page](Readme/AboutPage.png)

This page has nthLink information.

## Architecture / Source Code

### High Level overview

![nthlink windows architecture](Readme/nthlinkWindows_architecture.png)

There are two steps within connection.

1. After click connecting button, nthlink will get a configuration from Directory Server, which is
   used to connect to VPN server.

2. After get the configuration, [leaf](https://github.com/eycorsican/leaf) will connect to the VPN
   server, which is a VPN client library, written in Rust.

### Some of the uncommon codebase

For the best user experience, we will collect information for analysis. nthlink will collect web
page events, such as launch landing page and user click.


