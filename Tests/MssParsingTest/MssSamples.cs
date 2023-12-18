namespace MssParsingTest
{
    public static class MssSamples
    {
        // Classes
        public static readonly string EmptyClass = "types { class Volume { } }";
        public static readonly string BasicClass = "types { class Volume { Amount: float; } }";
        public static readonly string DualPropertyClass = "types { class Volume { Amount: float; Price: float; } }";
        public static readonly string ClassWithClassPropertyType = "types { class Volume { Amount: Volume; } }";

        // Externs
        public static readonly string ExternWithClassOnly = "types { extern Volume; }";
        public static readonly string ExternWithoutNamespace = "types { extern Volume: Project; }";
        public static readonly string ExternWithSingleNamespace = "types { extern Volume: Project::Namespace; }";
        public static readonly string ExterwWithMultipleNamespaces = "types { extern Volume: Project::Namespace.SubNamespace; }";

        public static readonly string EmptyService = "service Milking { }";
        public static readonly string BasicService =
@"types {
    class Volume {
        Amount: float;
    }
}
service Milking {
    database {
        root {
            Id: PK;
            MilkContainerId: FK;
            Milk: Volume;
        }

        entity MilkContainer {
            Id: PK;
            MilkContainerId: EK;
        }

        root.MilkContainerId }o--|| MilkContainer.Id;
    }

    aggregate {
        Id: EK;
        MilkContainerId: EK = root.MilkContainerId.MilkContainerId;
        Milk: Volume;
    }
}";
        public static readonly string BasicServiceWithoutTypes =
@"service Milking {
    database {
        root {
            Id: PK;
            MilkContainerId: FK;
            Milk: Volume;
        }

        entity MilkContainer {
            Id: PK;
            MilkContainerId: EK;
        }

        root.MilkContainerId }o--|| MilkContainer.Id;
    }

    aggregate {
        Id: EK;
        MilkContainerId: EK = root.MilkContainerId.MilkContainerId;
        Milk: Volume;
    }
}";
    }
}