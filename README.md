# onibara.logger

Implementation of simple microsoft logging to save logs on postgresql database.

Dowload it with Nuget Packages

https://www.nuget.org/packages/Onibara.Logger/

# To Use It:

Do it in your program.cs class:

using Onibara.Logger;

builder.Logging.AddDbLogger(options ⇒ { builder.Configuration.GetSection("Logging").GetSection("Database").GetSection("Options").Bind(options); });

and do it in your appsettings.json file

"Logging": { "Database": { "Options": { "ConnectionString": "your connection string", "LogFields": [ "LogLevel", "ThreadId", "EventId", "EventName", "ExceptionMessage", "ExceptionStackTrace", "ExceptionSource" ], "LogTable": "public.error" }, "LogLevel": { "Default": "Error", "Microsoft.AspNetCore": "Error"
} } }

Your table in Postgresql Database should be:

CREATE TABLE IF NOT EXISTS public.error ( errorid bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ), errorvalue character varying(2000) COLLATE pg_catalog."default", created timestamp without time zone, CONSTRAINT error_pkey PRIMARY KEY (errorid) )
