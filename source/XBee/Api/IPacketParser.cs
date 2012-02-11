namespace Gadgeteer.Modules.GHIElectronics.Api
{
    public interface IPacketParser
    {
        int Read(string context);
        int[] ReadRemainingBytes();
        int FrameDataBytesRead { get; }
        int RemainingBytes { get; }
        int BytesRead { get; }
        XBeePacketLength Length { get; }
        ApiId ApiId { get; }
	    // TODO move to util
	    XBeeAddress16 ParseAddress16();
	    XBeeAddress64 ParseAddress64();
    }
}