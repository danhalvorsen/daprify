namespace Mss.Types
{
    public class MssInvalidType : MssType
    {
        public MssInvalidType() : base("InvalidType") { }
        public override bool IsValid => false;
        public override string ToString() => "InvalidType";
    }
}