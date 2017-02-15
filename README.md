# Rollbar.NET

A .NET Rollbar Client for Xamarin Forms, based on the project code [Rollbar.NET](https://github.com/rollbar/Rollbar.NET)

## Prerequisites

In the Android and iOS projects, install the following package:
* Json.NET 9.0.1

In all PCL projects where the Rollbar plugin is added, installed the following nuget packages :
* Json.NET 9.0.1

## RollbarConfig

The `RollbarConfig` object allows you to configure the Rollbar library.

  <dl>
<dt>AccessToken
</dt>
<dd>The access token for your project, allows you access to the Rollbar API
</dd>
<dt>Endpoint
</dt>
<dd>The Rollbar API endpoint, defaults to https://api.rollbar.com/api/1/
</dd>
<dt>Environment
</dt>
<dd>Environment name, e.g. `"production"` or `"development"` defaults to `"production"`
</dd>
<dt>Enabled
</dt>
<dd>If set to false, errors will not be sent to Rollbar, defaults to `true`
</dd>
<dt>LogLevel
</dt>
<dd>The default level of the messages sent to Rollbar
</dd>
<dt>Transform
</dt>
<dd>Allows you to specify a transformation function to modify the payload before it is sent to Rollbar

```csharp
new RollbarConfig
{
    Transform = payload =>
    {
        payload.Data.Person = new Person
        {
            Id = 123,
            Username = "rollbar",
            Email = "user@rollbar.com"
        };
    }
}
```

</dd>
</dl>

## Person Data

You can set the current person data with a call to

```csharp
Rollbar.Current.PersonData(() => new Person
{
    Id = 123,
    Username = "rollbar",
    Email = "user@rollbar.com"
});
```

and this person will be attached to all future Rollbar calls.

## Model Data

You can set the current model with a call to

```csharp
RollbarDotNet.Data.SetModel("model device");
```

You can use the nuget plugin [Xam.Plugin.DeviceInfo](https://www.nuget.org/packages/Xam.Plugin.DeviceInfo/) to easily get the model.

## Application Version Data

You can set the current application version with a call to

```csharp
RollbarDotNet.Data.SetAppVersion("application version");
```

You can use the nuget plugin [Plugin.VersionTracking](https://www.nuget.org/packages/Plugin.VersionTracking/) to easily get the application version.

## OS Version Data

You can set the current OS version with a call to

```csharp
RollbarDotNet.Data.SetOSVersion("android version");
```

You can use the nuget plugin [Xam.Plugin.DeviceInfo](https://www.nuget.org/packages/Xam.Plugin.DeviceInfo/) to easily get the OS version.

## Advanced Usage

If you want more control over sending the data to Rollbar, you can fire up a `RollbarClient`
yourself, and make the calls directly. To get started you've got exactly one interesting class
to worry about: `RollbarPayload`. The class and the classes that compose the class cannot be
constructed without all mandatory arguments, and mandatory fields cannot be set.
Therefore, if you can construct a payload then it is valid for the purposes of
sending to Rollbar. To get the JSON to send to Rollbar just call
`RollbarPayload.ToJson` and stick it in the request body.

There are two other *particularly* interesting classes to worry about:
`RollbarData` and `RollbarBody`. `RollbarData` can be filled out as completely
or incompletely as you want, except for the `Environment` ("debug",
"production", "test", etc) and and `RollbarBody` fields. The `RollbarBody` is
where "what you're actually posting to Rollbar" lives. All the other fields on
`RollbarData` answer contextual questions about the bug like "who saw this
error" (`RollbarPerson`), "What HTTP request data can you give me about the
error (if it happened during an HTTP Request, of course)" (`RollbarRequest`),
"How severe was the error?" (`Level`). Anything you see on the
[rollbar api website](https://rollbar.com/docs/api/items_post/) can be found in
`Rollbar.NET`.

`RollbarBody` can be constructed one of 5 ways:

 1. With a class extending from `Exception`, which will automatically produce a
 `RollbarTrace` object, assigning it to the `Trace` field of the `RollbarBody`.
 2. With a class extending from `AggregateException`, which will automatically
 produce an array of `RollbarTrace` objects for each inner exception, assigning
 it to the `TraceChain` field of `RollbarBody`.
 3. With an actual array of `Exception` objects, which will automatically
 produce an array of `RollbarTrace` objects for each exception, assigning
 it to the `TraceChain` field of `RollbarBody`.
 4. With a `RollbarMessage` object, which consists of a string and any
 additional keys you wish to send along. It will be assigned to the `Message`
 field of `RollbarBody`.
 5. With a string, which should be formatted like an iOS crash report. This
 library has no way to verify if you've done this correctly, but if you pass in
 a string it will be wrapped in a dictionary and assigned to the `"raw"` key and
 assigned to the `CrashReport` field of `RollbarBody`

None of the fields on `RollbarBody` are updatable, and all null fields in
`Rollbar.NET` are left off of the final JSON payload.

## Basic Usage and Examples

### Xamarin.Forms Android

Initialize the rollbar plugin in the `MainActivity.cs` class

```csharp
[Init other plugins]
...
RollbarDotNet.Droid.Rollbar.Init(new RollbarConfig("SERVER_TOKEN"));
...
LoadApplication(new App());
```

### Xamarin.Forms iOS

Initialize the rollbar plugin in the `AppDelegate.cs` class

```csharp
[Init other plugins]
...
RollbarDotNet.iOS.Rollbar.Init(new RollbarConfig("SERVER_TOKEN"));
...
LoadApplication(new App());
```

### Xamarin.Forms PCL

```csharp
await Rollbar.Current.Report(ex, ErrorLevel.Error);
```
