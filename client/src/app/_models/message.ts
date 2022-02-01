
    export interface Message {
        id: number;
        userSenderId: number;
        userSenderUsername: string;
        userSenderPhotoUrl: string;
        userRecipientId: number;
        userRecipientUsername: string;
        userRecipientPhotoUrl: string;
        content: string;
        dateRead?: Date;
        messageSent: Date;
    }

