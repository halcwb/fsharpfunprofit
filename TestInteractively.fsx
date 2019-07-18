
// assumes nuget install Microsoft.AspNet.WebApi.OwinSelfHost has been run 
// so that assemblies are available under the current directory
#I __SOURCE_DIRECTORY__
#load ".paket/load/net471/main.group.fsx"

// sets the current directory to be same as the script directory
System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

open System
open Microsoft.Owin
open System.Web.Http
open System.Web.Http.Dispatcher
open System.Net.Http.Formatting

module OwinSelfHostSample =

    [<CLIMutable>]
    type Greeting = { Text : string }

    type GreetingController () =
        inherit ApiController()

        member this.Get () = { Text = "Hello" }

    type ApiRoute = { id : RouteParameter }
    

    type ControllerResolver () =
        inherit DefaultHttpControllerTypeResolver ()

        /// IMPORTANT: When running interactively, the controllers will not be found with error:
        /// "No type was found that matches the controller named 'XXX'."
        /// The fix is to override the ControllerResolver to use the current assembly
        override __.GetControllerTypes (assembliesResolver : IAssembliesResolver) =
            let t = typeof<Controllers.IHttpController>
            Reflection.Assembly.GetExecutingAssembly().GetTypes()
            |> Array.filter t.IsAssignableFrom
            :> Collections.Generic.ICollection<Type>

    type MyHttpConfiguration () as this =
        inherit HttpConfiguration ()

        let configureRoutes () =
            this.Routes.MapHttpRoute(
                name = "DefaultApi",
                routeTemplate = "api/{controller}/{id}",
                defaults = { id = RouteParameter.Optional }            
            ) |> ignore

        let configureJsonSerialization () =
            let jsonSettings = this.Formatters.JsonFormatter.SerializerSettings
            jsonSettings.Formatting <- Newtonsoft.Json.Formatting.Indented
