public static class EventCode
{
    public const byte PlayerMarksTileEventCode = 1;
    public const byte LoadNextMazeLevelEventCode = 2;
    public const byte LoadOverworldEventCode = 3;

    public const byte PlayerSendsMazeLevelInvitationEventCode = 5;
    public const byte PlayerRejectsMazeLevelInvitationEventCode = 6;
    public const byte PlayerCollidesWithMusicInstrumentCaseEventCode = 7;
    public const byte EnemyCollidesWithMusicInstrumentCaseEventCode = 8;
    public const byte HostLeavesGameEventCode = 9;
    public const byte UpdateRoomNameEventCode = 10;
    public const byte UpdateGameModeEventCode = 11;
}
