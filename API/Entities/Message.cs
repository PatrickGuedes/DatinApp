using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public AppUser UserSender { get; set; }
        public int UserSenderId { get; set; }
        public string UserSenderUsername { get; set; }

        public AppUser UserRecipient { get; set; }
        public int UserRecipientId { get; set; }
        public string UserRecipientUsername { get; set; }


        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;

        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}