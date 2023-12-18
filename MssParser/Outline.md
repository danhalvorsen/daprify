# Micro Service Generator

The idea is to generate a set of micro services from a domain language.
Automatically create the types, entities, dbcontext, etc. for a set of microservices.
Should include validation, test generation

will create a project per service
will output to xml schema and plantuml
the projects will build the classes from xml schema in a prebuild step

## BNF

```BNF
<Spec> ::= <SpecItem>+
<SpecItem> ::= <TypeSpec> | <Service>

<TypeSpec> ::= "types" "{" <TypeDef>* "}"
<TypeDef> ::= <Class> | <Extern>

<Class> ::= "class" <Identifier> "{" <ClassProperty>+ "}"
<ClassProperty> ::= <Identifier> ":" <PropertyType> ";"
<PropertyType> ::= <Identifier> | <BuiltInType>
<BuiltInType> ::= "int" | "float" | "bool" | "string";

<Extern> ::= "extern" <Identifier> ":" <Identifier> "::" <Access> ";"
<Access> ::= <Identifier> ["." <Identifier>]*

<Service> ::= "service" <Identifier> "{" <Database> <Aggregate> "}"
<Database> ::= "database" "{" <Root> [<Schema>]* "}"
<Root> ::= "root" "{" <EntityProperty>+ "}"
<Schema> ::= <Entity> | <Relation>

<Entity> ::= "entity" <Identifier> "{" <EntityProperty>+ "}"
<EntityProperty> ::= <Identifier> ":" <EntityPropertyType> ";"
<EntityPropertyType> ::= "PK" | "FK" | "EK" | <Identifier>;

<Relation> ::= <Access> <RelationStart> <RelationEnd> <Access> ";"
<RelationStart> ::= "|o-" | "||-" | "}o-" | "}-"
<RelationEnd> ::= "-o|" | "-||" | "-o{" | "-{";

<Aggregate> ::= "aggregate" "{" <AggregateProperty>+ "}"
<AggregateProperty> ::= <AggregatePropertyAuto> | <AggregatePropertyMapped>
<AggregatePropertyAuto> ::= <Identifier> ":" <AggregatePropertyType> ";"
<AggregatePropertyMapped> ::= <Identifier> ":" <AggregatePropertyType> "=" <Access> ";"
<AggregatePropertyType> ::= "EK" | <Identifier> | <ListType>
<ListType> ::= "List" "<" <ListContainedType> ">"
<ListContainedType> ::= "EK" | <Identifier>
```

## Domain Language Example

```
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
            When: TimeInterval;
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
        LivestockId: EK = root.Animal.LivestockId;
        MilkContainerId: EK = root.MilkContainer.MilkContainerId;
        When: TimeInterval;
        Milk: Volume;
        Fat: Nutrient;
        Lactose: Nutrient;
        Protein: Nutrient;
        CellCount: Cells;
        Viable: Viability;
    }
}
```
