# Remembering WWI App

![Remembering WWI](/images/rememberingwwi.png)

## Structure

The Remembering WWI app is developed with the Xamarin platform, founded by the engineers that created Mono, Mono for Android and MonoTouch, which are cross-platform implementations of the Common Language Infrastructure (CLI) and Common Language Specifications (often called Microsoft .NET).

With a C#-shared codebase, developers can use Xamarin tools to write native Android, iOS, and Windows apps with native user interfaces and share code across multiple platforms, including Windows and macOS.

![Structure](/images/structure.png)

## NARA (Portable class library) UI

Portable class library that contains the UI structure and common settings:
* **(Util)** Common app utilities
	* Settings for the environment that the app points to (Test/Production)
	* Font settings
	* …
* **(Views)** Xaml views
* **(Custom controls)** Controls used for custom renderers
* **(Content)** Static web content, such as loader control (deprecated) that are used on all of the supported platforms (iOS / Android).

![NARA (Portable class library) UI](/images/naraui.png)

## NARA.Common (Portable class library)

Portable class library that implements operations with the DB and communications with the web services. Database (SQLite) is used for storing local user data, content caching, cookie management.

Library consists of:
* **(Model)** Representation of the entities that the app operates with
* **(Repository)** Implemented operations on the entities with the DB
* **(Service)** Logic for communication with the web services
* **(Util)** Common settings, such as basic web service auth., encryption and decryption mechanisms, token verifications

![NARA.Common (Portable class library)](/images/naracommon.png)

## NARA.Droid (Xamarin.Droid project)

Xamarin.Android project is used to create native Android applications using the same UI controls that are used in Java, except with use of the C# language with .NET Base Class Library (BCL).

Project consists of:
* **(Assets & Content & Embedded)** Static web content, such as loader control and error page that is presented when an error occurs while retrieving or presenting web content
* **(Renderers)** Custom renderers developed, that translates the custom Xamarin.Forms controls into native ones.
* **(Resources)** Images included in the project
* **(Util)** Platform specific code, such as implementation of reachability (network access)
* Activities (**Main ->** main entry point of the application, Splashscreeen activity)

![NARA.Droid (Xamarin.Droid project)](/images/naradroid.png)

## NARA.iOS (Xamarin.iOS project)

Xamarin.iOS is used to create native iOS applications using the same UI controls that are available in Objective-C and Xcode, except with the use of the C# language, .NET Base Class Library (BCL).

Project consists of:
* (Content) Static web content, such as loader control and error page that is presented when an error occurs while retrieving or presenting web content
* (Renderers) Custom renderers developed, that translates the custom Xamarin.Forms controls into native ones.
* (Resources) Images included in the project
* (Util) Platform specific code, such as implementation of reachability (network access)
* Main entry point of the application (Main.cs with AppDelegate.cs)

![NARA.iOS (Xamarin.iOS project)](/images/naraios.png)
