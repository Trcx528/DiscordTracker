using Discord.WebSocket;
using DiscordTracker.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTracker.Commands
{
    public static class Quotes
    {
        public static async Task Execute(SocketMessage message)
        {
            // "|quote add this is a test <@!227328871270318080>"
            var sections = message.Content.Split(' ');
            int id;
            if (sections.Count() == 1)
            {
                var rand = new Random();
                var q = Program._db.Quotes.Include(a => a.Author).Skip(rand.Next(0, Program._db.Quotes.Count())).First();
                await message.Channel.SendMessageAsync(q.DiscordDisplayMessage);
            }
            else if (sections[1].ToLower() == "add")
                await Add(message, sections);

            else if (sections[1].ToLower() == "delete")
                await Delete(message, sections);

            else if (sections[1].ToLower() == "search")
                await Search(message, sections);

            else if (int.TryParse(sections[1], out id))
                await message.Channel.SendMessageAsync(Program._db.Quotes.Find(id).DiscordDisplayMessage);
        }

        private static async Task Add(SocketMessage message, string[] parts)
        {
            if (message.MentionedUsers.Count() != 1)
            {
                await message.Channel.SendMessageAsync("Please include only one author with an @");
                return;
            }
            await DiscordUser.CreateOrGetAsync(message.Author);
            await DiscordUser.CreateOrGetAsync(message.MentionedUsers.First());

            var quoteText = parts.Skip(2).Where(s => !s.Contains(message.MentionedUsers.First().Mention)).Aggregate("", (c, n) => c + " " + n).Trim();
            var quote = new Quote() { AddedById = message.Author.Id, AuthorId = message.MentionedUsers.First().Id, Created = DateTime.Now, QuoteText = quoteText };
            Program._db.Add(quote);
            await Program._db.SaveChangesAsync();
            await message.Channel.SendMessageAsync($"Quote {quote.Id} created!");
        }

        private static async Task Delete(SocketMessage message, string[] parts)
        {
            if(!message.Author.IsAdmin())
            {
                await message.Channel.SendMessageAsync("You do not have permission to do that");
                return;
            }

            int id;
            if (parts.Count() == 3 && int.TryParse(parts[2], out id))
            {
                Program._db.Remove(Program._db.Quotes.Find(id));
                await Program._db.SaveChangesAsync();
                await message.Channel.SendMessageAsync("Quote Deleted");
            }
            else
            {
                await message.Channel.SendMessageAsync("Invalid Delete Command");
            }

        }

        private static async Task Search(SocketMessage message, string[] parts)
        {
            var results = Program._db.Quotes.Where(i => true);
            foreach (var part in parts.Skip(2))
            {
                results = results.Where(q => q.QuoteText.Contains(part));
            }

            var sb = new StringBuilder();
            foreach (var row in await results.ToListAsync())
            {
                sb.Append($"{ row.Id.ToString().PadLeft(4)} | {row.QuoteText}\n");
            }

            await message.Channel.SendMessageAsync("```" + sb.ToString() + "```");
        }
    }
}
