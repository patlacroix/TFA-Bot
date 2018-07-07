using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace TFABot.Dialler
{
    public interface IDialler
    {
        Task CallAsync(String Name, String Number, DSharpPlus.Entities.DiscordChannel ChBotAlert = null);
	}
    
}
