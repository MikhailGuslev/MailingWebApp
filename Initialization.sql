create table 'User'
(
    UserId integer not null,
    Email text not null
);

create table Plugin
(
    PluginId integer not null,
    Name text not null,
    Comment text not null,
    Data blob not null,
    CreatedDate datetime not null,
    UpdatedDate datetime not null
);

create table ModelProvider
(
    ModelProviderId integer not null,
    ModelProviderTypeName text not null,

    PluginId integer not null
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
    ActivationInterval integer not null,

    EmailSendingId integer not null
);