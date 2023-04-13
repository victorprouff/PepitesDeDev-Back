DO
$do$
BEGIN
        IF
EXISTS ( SELECT 1 FROM pg_roles WHERE rolname = 'cloudsqlsuperuser' ) THEN
            DO
            $$
BEGIN
EXECUTE format('GRANT ALL ON DATABASE %I  TO %I', current_database(), 'cloudsqlsuperuser');
END;
            $$;
            GRANT ALL PRIVILEGES ON ALL
TABLES IN SCHEMA public TO cloudsqlsuperuser;
            GRANT ALL PRIVILEGES ON ALL
SEQUENCES IN SCHEMA public TO cloudsqlsuperuser;
            GRANT ALL PRIVILEGES ON ALL
FUNCTIONS IN SCHEMA public TO cloudsqlsuperuser;
            GRANT ALL
ON SCHEMA public  TO cloudsqlsuperuser;
END IF;
END
$do$;