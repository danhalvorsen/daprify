namespace Mss
{
    public abstract class MssRelationKind
    {
        public abstract override string ToString();
        public abstract string FromString();
    }

    public class MssRelationKindZeroOrOne : MssRelationKind
    {
        public override string ToString() => "-o|";
        public override string FromString() => "|o-";
    }

    public class MssRelationKindOne : MssRelationKind
    {
        public override string ToString() => "-||";
        public override string FromString() => "||-";
    }

    public class MssRelationKindZeroOrMany : MssRelationKind
    {
        public override string ToString() => "-o{";
        public override string FromString() => "}o-";
    }

    public class MssRelationKindMany : MssRelationKind
    {
        public override string ToString() => "-{";
        public override string FromString() => "}-";
    }
}