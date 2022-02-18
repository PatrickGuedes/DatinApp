using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }

        public int UserSenderId { get; set; }
        public string UserSenderUsername { get; set; }
        public string UserSenderPhotoUrl { get; set; }

        public int UserRecipientId { get; set; }
        public string UserRecipientUsername { get; set; }
        public string UserRecipientPhotoUrl { get; set; }

        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }

        [JsonIgnore]
        public bool RecipientDeleted { get; set; }
        [JsonIgnore]
        public bool SenderDeleted { get; set; }
    }
}