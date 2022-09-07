create table MeterReadingsPeriodDetails (
    MeterReadingsPeriodDetailsId  integer  not null,
    ProvidedServiceName           text     not null,
    ServiceProviderName           text     not null,
    MeteringDevice                text     not null,
    StartTakingReadings           datetime not null,
    EndTakingReadings             datetime not null,

    UserId                        integer not null
);

create table 'User'
(
    UserId integer not null,
    Email text not null
);

create table PluginAssembly
(
    PluginAssemblyId integer not null,
    Name text not null,
    Comment text not null,
    Settings text not null,
    Data blob not null,
    CreatedDate datetime not null,
    UpdatedDate datetime not null
);

create table ModelProvider
(
    ModelProviderId integer not null,
    PluginAssemblyId integer not null
);

create table MessageTemplate
(
    MessageTemplateId integer not null,
    Subject text not null,
    Body text not null,
    ContentType text not null,
    IsBodyStatic boolean not null,
    IsSubjectStatic boolean not null,

    ModelProviderId integer null,

    check (ContentType in ("PlainText", "Html"))
);

create table EmailSending
(
    SendingId integer not null,
    Name text not null,
    Recipients text not null,

    MessageTemplateId integer not null
);

create table EmailSendingSchedule
(
    EmailSendingScheduleId integer not null,
    ActivationTimePoint datetime not null,
    DeactivationTimePoint datetime not null,
    RecurrenceActivation text not null,
    LastActivation datetime not null,

    EmailSendingId integer not null
);