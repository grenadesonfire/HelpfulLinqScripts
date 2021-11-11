<Query Kind="Program">
  <NuGetReference>Discord.Net</NuGetReference>
  <Namespace>Discord</Namespace>
  <Namespace>Discord.API</Namespace>
  <Namespace>Discord.Audio</Namespace>
  <Namespace>Discord.Audio.Streams</Namespace>
  <Namespace>Discord.Commands</Namespace>
  <Namespace>Discord.Commands.Builders</Namespace>
  <Namespace>Discord.Net</Namespace>
  <Namespace>Discord.Net.Converters</Namespace>
  <Namespace>Discord.Net.Queue</Namespace>
  <Namespace>Discord.Net.Rest</Namespace>
  <Namespace>Discord.Net.Udp</Namespace>
  <Namespace>Discord.Net.WebSockets</Namespace>
  <Namespace>Discord.Rest</Namespace>
  <Namespace>Discord.Webhook</Namespace>
  <Namespace>Discord.WebSocket</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//https://docs.stillu.cc/guides/getting_started/first-bot.html
static string TOKEN = "ODMxMzU2NDIwNjUyNzkzODk3.YHUDAA.1hM5moJz8LAtimvM_0Kamt9GAKI";
private DiscordSocketClient _client;

private CommandService _commands;

void Main()
{
	MainAsync().GetAwaiter().GetResult();
}

// You can define other methods, fields, classes and namespaces here
public async Task MainAsync()
{
	_client = new DiscordSocketClient();
	_client.Log += Log;
	
	_commands = new CommandService();
	
	await _client.LoginAsync(TokenType.Bot, TOKEN);
	await _client.StartAsync();
	
	await Task.Delay(-1);
}

private Task Log(LogMessage msg)
{
	Console.WriteLine(msg.ToString());
	return Task.CompletedTask;
}

private async Task InstallCommandsAsync() 
{
	_client.MessageReceived += HandleCommandAsync;
	//https://docs.stillu.cc/guides/commands/intro.html
	await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
}
private async Task HandleCommandAsync(SocketMessage arg)
{
	var msg = arg as SocketUserMessage;
	
	if(msg ==null) return;
	
	int argPos = 0;

	if (!(msg.HasCharPrefix('!', ref argPos) ||
			msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
			msg.Author.IsBot)
		return;

	// Create a WebSocket-based command context based on the message
	var context = new SocketCommandContext(_client, msg);

	// Execute the command with the command context we just
	// created, along with the service provider for precondition checks.
	await _commands.ExecuteAsync(
		context: context,
		argPos: argPos,
		services: null);
}
