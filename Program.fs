open Data
open DSharpPlus
open DSharpPlus.CommandsNext
open DSharpPlus.EventArgs
open GoodBot
open System.Threading.Tasks
open System
open Emzi0767.Utilities
open DSharpPlus.Entities

module Core =
    let rand = Random()

    let clientReady (dis: DiscordClient) _ =
        match db.Status with
        | Some (name, activityType) -> dis.UpdateStatusAsync(DiscordActivity(name, activityType))
        | None -> Task.CompletedTask

    let messageCreated (dis: DiscordClient) (e: MessageCreateEventArgs) =
        if Seq.contains dis.CurrentUser e.MentionedUsers then
            let responseNum = rand.Next(db.Responses.Length)
            dis.SendMessageAsync(e.Channel, db.Responses.[responseNum]) :> Task
        elif e.Author.Id = 235148962103951360UL then
            dis.SendMessageAsync(e.Channel, "Carl is a cuck") :> Task
        else
            Task.CompletedTask

    let typingStart (dis: DiscordClient) (e: TypingStartEventArgs) =
        if rand.Next(1000) = 0 then
            dis.SendMessageAsync(e.Channel, sprintf "shut up <@%u>" e.User.Id) :> Task
        else
            Task.CompletedTask

    let discordConfig =
        DiscordConfiguration(Token = config.DiscordToken, TokenType = TokenType.Bot, AutoReconnect = true)

    let discord = new DiscordClient(discordConfig)

    discord.add_Ready (AsyncEventHandler<_, _>(clientReady))
    discord.add_MessageCreated (AsyncEventHandler<_, _>(messageCreated))
    discord.add_TypingStarted (AsyncEventHandler<_, _>(typingStart))

    let commandConfig =
        CommandsNextConfiguration(StringPrefixes = [ config.CommandPrefix ])

    let commands = discord.UseCommandsNext(commandConfig)
    commands.RegisterCommands<Commands>()

[<EntryPoint>]
let main _ =
    Core.discord.ConnectAsync()
    |> Async.AwaitTask
    |> Async.RunSynchronously

    Task.Delay(-1)
    |> Async.AwaitTask
    |> Async.RunSynchronously

    0 // return an integer exit code
