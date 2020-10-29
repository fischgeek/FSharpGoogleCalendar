namespace FSharpGoogleCalendar

open Google.Apis.Auth.OAuth2
open Google.Apis.Calendar.v3
open Google.Apis.Services
open Google.Apis.Util.Store
open System
open System.IO
open System.Threading

module GoogleCalendar =

    let private scopes = [| CalendarService.Scope.CalendarReadonly |]
    let private credsFile = @"c:\dev\config\google-calendar-credentials.json"
    let private credPath = "token.json"

    let private Authenticate() =
        let stream = new FileStream(credsFile, FileMode.Open, FileAccess.Read)
        let credential =
            (GoogleWebAuthorizationBroker.AuthorizeAsync
                (GoogleClientSecrets.Load(stream).Secrets, scopes, "user", CancellationToken.None, new FileDataStore(credPath, true))).Result
        credential

    let private CreateGoogleCalendarService credential =
        let mutable baseClientService = new BaseClientService.Initializer()
        baseClientService.HttpClientInitializer <- credential
        baseClientService.ApplicationName <- "Application Name"
        new CalendarService(baseClientService)

    let private CreateRequest service =
        let mutable request = new EventsResource.ListRequest(service, "primary")
        request.TimeMin <- Nullable DateTime.Now
        request.ShowDeleted <- Nullable false
        request.SingleEvents <- Nullable true
        request.MaxResults <- Nullable 100
        request.OrderBy <- Nullable EventsResource.ListRequest.OrderByEnum.StartTime
        request

    let private GetEvents(request: EventsResource.ListRequest) =
        try
            let events = request.Execute()
            events.Items
            |> Seq.toList
            |> function
            | [] -> None
            | list ->
                list |> Seq.iter (fun event -> printfn "%s" event.Summary)
                Some list
        with ex -> failwith ex.Message

    let GetEventList() =
        Authenticate()
        |> CreateGoogleCalendarService
        |> CreateRequest
        |> GetEvents
    ()
