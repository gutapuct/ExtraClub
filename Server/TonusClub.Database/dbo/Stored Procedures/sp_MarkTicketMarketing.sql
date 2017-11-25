create procedure sp_MarkTicketMarketing(@ticketId uniqueidentifier)
as
insert into SyncMetadata..NotifiedMessages select NEWID(), @ticketId, getdate()