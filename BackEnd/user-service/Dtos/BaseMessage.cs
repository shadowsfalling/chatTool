public class BaseMessage
{
    public string Action { get; set; }  // Z.B. "ValidateUser", "CreateUser"
    public string Payload { get; set; }  // Die eigentlichen Daten, die die Aktion betreffen (z.B. User-Daten)
}