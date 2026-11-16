namespace ArtConnect.Entities
{
    public enum ArtworkStatus { Available = 1, Sold = 2 }

    public enum RequestStatus { Open = 1, Accepted = 2, Closed = 3, Cancelled = 4 }

    public enum ProposalStatus { Pending = 1, Accepted = 2, Rejected = 3, Withdrawn = 4 }

    public enum OrderType { Purchase = 1, Commission = 2 }

    public enum OrderStatus { Pending = 1, InProgress = 2, Delivered = 3, Cancelled = 4 }
}
