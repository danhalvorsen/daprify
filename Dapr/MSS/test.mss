types {
    class Volume {
        Amount: float;
    }

    class Nutrient {
        Percent: float;
    }

    class Cells {
        Count: int;
    }

    class Viability {
        Viable: bool;
    }

    extern TimeInterval: Types::Types.Time;
    extern TestClass: TestProject::TestNamespace;
}

// line-comment
/* multi
   line
   comment */

service Milking {
    database {
        root {
            Id: PK;
            AnimalId: FK;
            MilkContainerId: FK;
            Milk: Volume;
            Fat: Nutrient;
            Lactose: Nutrient;
            Protein: Nutrient;
            CellCount: Cells;
            Viable: Viability;
        }

        entity Animal {
            Id: PK;
            LivestockId: EK;
        }

        entity MilkContainer {
            Id: PK;
            MilkContainerId: EK;
        }

        root.AnimalId }o--|| Animal.Id;
        root.MilkContainerId }o--|| MilkContainer.Id;
    }

    aggregate {
        Id: EK;
        LivestockId: EK = root.AnimalId.LivestockId;
        MilkContainerId: EK = root.MilkContainerId.MilkContainerId;
        Milk: Volume;
        Fat: Nutrient;
        Lactose: Nutrient;
        Protein: Nutrient;
        CellCount: Cells;
        Viable: Viability;
    }
}

types {
    extern TimeStamp: Types::Types.Time;

    class OrderNumber {
        Number: string;
    }

    class Price {
        Amount: float;
    }
}

service LivestockOrder {
    database {
        root{
            Id: PK;
            CustomerId: FK;
            OrderNo: OrderNumber;
            Price: Price;
            Payed: bool;
        }

        entity Animal {
            Id: PK;
            OrderId: FK;
            LivestockId: EK;
        }

        entity Customer {
            Id: PK;
            StakeholderId: EK;
        }

        root.Id ||--{ Animal.OrderId;
        root.CustomerId }o--|| Customer.Id;
    }

    aggregate {
        Id: EK;
        OrderNo: OrderNumber;
        StakeholderId: EK = root.CustomerId.StakeholderId;
        LiveStock: List<EK> = Animal.LivestockId;
        Price: Price;
        Payed: bool;
    }
}