# Remembering WWI App

![Remembering WWI](/ww1/images/rememberingwwi.png)

## About the App

The National Archives and Records Administration (NARA) has recently launched Remembering WWI, an iPad and Android application that invites audiences to explore, collaborate, and engage with the our extensive collection of digitized World War I moving and still images. These include photographs and films originally shot by the US Signal Corps on behalf of various armed forces units in between 1914 and 1920.

Remembering WWI also invites people nationwide to contribute their own stories and play a part in the centennial commemoration of the US entry into World War I. Using the archival content within the app, the public can create their own collections and build and share new narratives around the people, events, and themes they're exploring. Thematic collections from NARA, Library of Congress, and the Smithsonian are featured to serve as inspiration or provide jumping off points for content discovery and reuse.

## Structure

The Remembering WWI app is developed with the Xamarin platform, founded by the engineers that created Mono, Mono for Android and MonoTouch, which are cross-platform implementations of the Common Language Infrastructure (CLI) and Common Language Specifications (often called Microsoft .NET).

With a C#-shared codebase, developers can use Xamarin tools to write native Android, iOS, and Windows apps with native user interfaces and share code across multiple platforms, including Windows and macOS.

![Structure](/ww1/images/structure.png)

## NARA (Portable class library) UI

Portable class library that contains the UI structure and common settings:
* **(Util)** Common app utilities
	* Settings for the environment that the app points to (Test/Production)
	* Font settings
	* …
* **(Views)** Xaml views
* **(Custom controls)** Controls used for custom renderers
* **(Content)** Static web content, such as loader control (deprecated) that are used on all of the supported platforms (iOS / Android).

![NARA (Portable class library) UI](/ww1/images/naraui.png)

## NARA.Common (Portable class library)

Portable class library that implements operations with the DB and communications with the web services. Database (SQLite) is used for storing local user data, content caching, cookie management.

Library consists of:
* **(Model)** Representation of the entities that the app operates with
* **(Repository)** Implemented operations on the entities with the DB
* **(Service)** Logic for communication with the web services
* **(Util)** Common settings, such as basic web service auth., encryption and decryption mechanisms, token verifications

![NARA.Common (Portable class library)](/ww1/images/naracommon.png)

## NARA.Droid (Xamarin.Droid project)

Xamarin.Android project is used to create native Android applications using the same UI controls that are used in Java, except with use of the C# language with .NET Base Class Library (BCL).

Project consists of:
* **(Assets & Content & Embedded)** Static web content, such as loader control and error page that is presented when an error occurs while retrieving or presenting web content
* **(Renderers)** Custom renderers developed, that translates the custom Xamarin.Forms controls into native ones.
* **(Resources)** Images included in the project
* **(Util)** Platform specific code, such as implementation of reachability (network access)
* Activities (**Main ->** main entry point of the application, Splashscreeen activity)

![NARA.Droid (Xamarin.Droid project)](/ww1/images/naradroid.png)

## NARA.iOS (Xamarin.iOS project)

Xamarin.iOS is used to create native iOS applications using the same UI controls that are available in Objective-C and Xcode, except with the use of the C# language, .NET Base Class Library (BCL).

Project consists of:
* (Content) Static web content, such as loader control and error page that is presented when an error occurs while retrieving or presenting web content
* (Renderers) Custom renderers developed, that translates the custom Xamarin.Forms controls into native ones.
* (Resources) Images included in the project
* (Util) Platform specific code, such as implementation of reachability (network access)
* Main entry point of the application (Main.cs with AppDelegate.cs)

![NARA.iOS (Xamarin.iOS project)](/ww1/images/naraios.png)
