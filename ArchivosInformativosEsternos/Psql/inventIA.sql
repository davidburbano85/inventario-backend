---- =========================================================
---- 1. EXTENSIONES
---- =========================================================
--CREATE EXTENSION IF NOT EXISTS pgcrypto;

---- =========================================================
---- 2. TABLAS BASE
---- =========================================================

---- EMPRESAS (SIN activo)
--CREATE TABLE empresas (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  nombre TEXT NOT NULL,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  CONSTRAINT empresas_nombre_check CHECK (length(trim(nombre)) > 0)
--);

---- USUARIOS (SIN activo)
--CREATE TABLE usuarios (
--  id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
--  nombre TEXT,
--  telefono TEXT,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now())
--);

---- USUARIOS EMPRESAS
--CREATE TABLE usuarios_empresas (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE,
--  usuario_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
--  rol TEXT NOT NULL DEFAULT 'usuario',
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT usuarios_empresas_unique UNIQUE (empresa_id, usuario_id),
--  CONSTRAINT usuarios_empresas_rol_check CHECK (rol IN ('usuario', 'admin', 'superadmin'))
--);

---- ALMACENES
--CREATE TABLE almacenes (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE,
--  nombre TEXT NOT NULL,
--  ubicacion TEXT,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT almacenes_empresa_nombre_unique UNIQUE (empresa_id, nombre)
--);

---- CATEGORIAS
--CREATE TABLE categorias (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE,
--  nombre TEXT NOT NULL,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT categorias_empresa_nombre_unique UNIQUE (empresa_id, nombre),
--  CONSTRAINT categorias_nombre_check CHECK (length(trim(nombre)) > 0)
--);

---- PRODUCTOS
--CREATE TABLE productos (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE,
--  categoria_id UUID REFERENCES categorias(id),
--  nombre TEXT NOT NULL,
--  codigo_sku TEXT,
--  precio_venta NUMERIC(12,2) NOT NULL,
--  precio_compra NUMERIC(12,2),
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT productos_empresa_sku_unique UNIQUE (empresa_id, codigo_sku),
--  CONSTRAINT productos_nombre_check CHECK (length(trim(nombre)) > 0),
--  CONSTRAINT productos_precio_venta_check CHECK (precio_venta >= 0),
--  CONSTRAINT productos_precio_compra_check CHECK (precio_compra IS NULL OR precio_compra >= 0)
--);

---- STOCK ACTUAL
--CREATE TABLE stock_actual (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE,
--  producto_id UUID NOT NULL REFERENCES productos(id) ON DELETE CASCADE,
--  almacen_id UUID NOT NULL REFERENCES almacenes(id) ON DELETE CASCADE,
--  cantidad NUMERIC DEFAULT 0,
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT stock_actual_unique UNIQUE (empresa_id, producto_id, almacen_id),
--  CONSTRAINT stock_actual_cantidad_check CHECK (cantidad >= 0)
--);

---- MOVIMIENTOS INVENTARIO
--CREATE TABLE movimientos_inventario (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE,
--  producto_id UUID NOT NULL REFERENCES productos(id),
--  almacen_id UUID NOT NULL REFERENCES almacenes(id),
--  usuario_id UUID REFERENCES auth.users(id),
--  tipo TEXT NOT NULL,
--  cantidad NUMERIC NOT NULL,
--  motivo TEXT,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT movimientos_tipo_check CHECK (tipo IN ('entrada', 'salida', 'ajuste')),
--  CONSTRAINT movimientos_cantidad_check CHECK (cantidad > 0)
--);

---- PROVEEDORES
--CREATE TABLE proveedores (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE,
--  nombre TEXT NOT NULL,
--  contacto TEXT,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT proveedores_empresa_nombre_unique UNIQUE (empresa_id, nombre),
--  CONSTRAINT proveedores_nombre_check CHECK (length(trim(nombre)) > 0)
--);

---- CLIENTES
--CREATE TABLE clientes (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE,
--  nombre TEXT NOT NULL,
--  contacto TEXT,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT clientes_empresa_nombre_unique UNIQUE (empresa_id, nombre),
--  CONSTRAINT clientes_nombre_check CHECK (length(trim(nombre)) > 0)
--);

---- COMPRAS
--CREATE TABLE compras (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id),
--  proveedor_id UUID REFERENCES proveedores(id),
--  total NUMERIC(12,2) NOT NULL,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT compras_total_check CHECK (total >= 0)
--);

---- COMPRAS DETALLE
--CREATE TABLE compras_detalle (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id),
--  compra_id UUID NOT NULL REFERENCES compras(id) ON DELETE CASCADE,
--  producto_id UUID NOT NULL REFERENCES productos(id),
--  cantidad NUMERIC NOT NULL,
--  precio NUMERIC NOT NULL,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT compras_detalle_cantidad_check CHECK (cantidad > 0),
--  CONSTRAINT compras_detalle_precio_check CHECK (precio >= 0)
--);

---- VENTAS
--CREATE TABLE ventas (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id),
--  cliente_id UUID REFERENCES clientes(id),
--  total NUMERIC(12,2) NOT NULL,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT ventas_total_check CHECK (total >= 0)
--);

---- VENTAS DETALLE
--CREATE TABLE ventas_detalle (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id),
--  venta_id UUID NOT NULL REFERENCES ventas(id) ON DELETE CASCADE,
--  producto_id UUID NOT NULL REFERENCES productos(id),
--  cantidad NUMERIC NOT NULL,
--  precio NUMERIC NOT NULL,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT ventas_detalle_cantidad_check CHECK (cantidad > 0),
--  CONSTRAINT ventas_detalle_precio_check CHECK (precio >= 0)
--);

---- RESUMEN DIARIO INVENTARIO
--CREATE TABLE resumen_diario_inventario (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id),
--  producto_id UUID REFERENCES productos(id),
--  fecha DATE NOT NULL,
--  total_ventas NUMERIC DEFAULT 0,
--  total_compras NUMERIC DEFAULT 0,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE,
--  CONSTRAINT resumen_diario_unique UNIQUE (empresa_id, producto_id, fecha),
--  CONSTRAINT resumen_ventas_check CHECK (total_ventas >= 0),
--  CONSTRAINT resumen_compras_check CHECK (total_compras >= 0)
--);

--ALTER TABLE resumen_diario_inventario
--ADD COLUMN almacen_id UUID;

--ALTER TABLE resumen_diario_inventario
--DROP CONSTRAINT IF EXISTS resumen_diario_unique;

--ALTER TABLE resumen_diario_inventario
--ADD CONSTRAINT resumen_diario_unique
--UNIQUE (empresa_id, almacen_id, producto_id, fecha);




---- LOGS SISTEMA
--CREATE TABLE logs_sistema (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--  empresa_id UUID NOT NULL REFERENCES empresas(id),
--  usuario_id UUID NOT NULL REFERENCES auth.users(id),
--  accion TEXT,
--  detalle TEXT,
--  created_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  updated_at TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--  activo BOOLEAN DEFAULT TRUE
--);
---- ==========================
---- modificar tabla Logs_sistem
----=========================

--ALTER TABLE logs_sistema
--ADD COLUMN IF NOT EXISTS tabla_afectada TEXT;

--ALTER TABLE logs_sistema
--ADD COLUMN IF NOT EXISTS registro_id UUID;

--ALTER TABLE logs_sistema
--ADD COLUMN IF NOT EXISTS operacion TEXT;

--ALTER TABLE logs_sistema
--ADD COLUMN IF NOT EXISTS datos JSONB;

--ALTER TABLE logs_sistema
--ALTER COLUMN usuario_id DROP NOT NULL;


---- monitore resumendiario 

--CREATE TABLE IF NOT EXISTS monitoreo_resumen_inventario (
--    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

--    empresa_id UUID,
--    almacen_id UUID,

--    tipo_proceso TEXT NOT NULL, 
--    -- 'diario' | 'rango' | 'rebuild'

--    fecha_desde DATE,
--    fecha_hasta DATE,

--    estado TEXT NOT NULL, 
--    -- 'en_proceso' | 'exitoso' | 'fallido' | 'reintentado_exitoso'

--    registros_procesados INTEGER DEFAULT 0,

--    mensaje TEXT,

--    fecha_inicio TIMESTAMPTZ DEFAULT timezone('America/Bogota', now()),
--    fecha_fin TIMESTAMPTZ,

--    creado_en TIMESTAMPTZ DEFAULT timezone('America/Bogota', now())
--);



---- =========================================================
---- 3. FUNCIONES
---- =========================================================

--CREATE OR REPLACE FUNCTION pertenece_empresa(emp_id UUID)
--RETURNS BOOLEAN
--LANGUAGE plpgsql
--STABLE
--SECURITY DEFINER
--AS $$
--BEGIN
--  IF auth.uid() IS NULL THEN
--    RETURN FALSE;
--  END IF;

--  RETURN EXISTS (
--    SELECT 1 FROM usuarios_empresas ue
--    WHERE ue.usuario_id = auth.uid()
--      AND ue.empresa_id = emp_id
--  );
--END;
--$$;

--CREATE OR REPLACE FUNCTION actualizar_stock()
--RETURNS TRIGGER AS $$
--BEGIN
--  IF NEW.tipo = 'entrada' THEN
--    UPDATE stock_actual
--    SET cantidad = cantidad + NEW.cantidad
--    WHERE producto_id = NEW.producto_id
--      AND almacen_id = NEW.almacen_id;
--  ELSE
--    UPDATE stock_actual
--    SET cantidad = cantidad - NEW.cantidad
--    WHERE producto_id = NEW.producto_id
--      AND almacen_id = NEW.almacen_id;
--  END IF;

--  RETURN NEW;
--END;
--$$ LANGUAGE plpgsql;

--CREATE OR REPLACE FUNCTION log_accion()
--RETURNS TRIGGER AS $$
--BEGIN
--  INSERT INTO logs_sistema (empresa_id, usuario_id, accion, detalle)
--  VALUES (NEW.empresa_id, auth.uid(), TG_OP, 'Cambio en tabla');
--  RETURN NEW;
--END;
--$$ LANGUAGE plpgsql;

--CREATE OR REPLACE FUNCTION handle_new_user()
--RETURNS TRIGGER AS $$
--BEGIN
--  INSERT INTO public.usuarios (id, nombre, telefono)
--  VALUES (
--    NEW.id,
--    NEW.raw_user_meta_data ->> 'nombre',
--    NEW.raw_user_meta_data ->> 'telefono'
--  );
--  RETURN NEW;
--END;
--$$ LANGUAGE plpgsql;

--CREATE OR REPLACE FUNCTION crear_usuario()
--RETURNS TRIGGER AS $$
--BEGIN
--  INSERT INTO public.usuarios (id)
--  VALUES (NEW.id);

--  RETURN NEW;
--END;
--$$ LANGUAGE plpgsql SECURITY DEFINER;



---- ==========================  
----   funcion resumen diario almacen_id
----==================================
--CREATE OR REPLACE FUNCTION generar_resumen_diario_inventario()
--RETURNS void
--LANGUAGE plpgsql
--AS $$
--DECLARE
--    total_registros INTEGER := 0;
--    fecha_proceso DATE := CURRENT_DATE;
--BEGIN

--    -- 🟡 Registrar inicio en nuevo monitoreo
--    INSERT INTO monitoreo_resumen_inventario (
--        empresa_id,
--        almacen_id,
--        tipo_proceso,
--        fecha_desde,
--        fecha_hasta,
--        estado,
--        mensaje
--    )
--    SELECT
--        NULL,
--        NULL,
--        'diario',
--        fecha_proceso,
--        fecha_proceso,
--        'en_proceso',
--        'Inicio batch diario';

--    -- 🚀 Proceso principal
--    WITH upsert AS (
--        INSERT INTO resumen_diario_inventario
--        (
--            empresa_id,
--            almacen_id,
--            producto_id,
--            fecha,
--            total_ventas,
--            total_compras,
--            created_at,
--            updated_at,
--            activo
--        )
--        SELECT
--            m.empresa_id,
--            m.almacen_id,
--            m.producto_id,
--            DATE(m.created_at),
--            SUM(CASE WHEN m.tipo = 'salida' THEN m.cantidad ELSE 0 END),
--            SUM(CASE WHEN m.tipo = 'entrada' THEN m.cantidad ELSE 0 END),
--            timezone('America/Bogota', now()),
--            timezone('America/Bogota', now()),
--            true
--        FROM movimientos_inventario m
--        WHERE m.activo = true
--        GROUP BY
--            m.empresa_id,
--            m.almacen_id,
--            m.producto_id,
--            DATE(m.created_at)
--        ON CONFLICT (empresa_id, almacen_id, producto_id, fecha)
--        DO UPDATE SET
--            total_ventas = EXCLUDED.total_ventas,
--            total_compras = EXCLUDED.total_compras,
--            updated_at = timezone('America/Bogota', now())
--        RETURNING 1
--    )
--    SELECT COUNT(*) INTO total_registros FROM upsert;

--    -- ✅ Marcar éxito
--    UPDATE monitoreo_resumen_inventario
--    SET
--        estado = 'exitoso',
--        registros_procesados = total_registros,
--        mensaje = 'Batch ejecutado correctamente',
--        fecha_fin = timezone('America/Bogota', now())
--    WHERE tipo_proceso = 'diario'
--      AND fecha_desde = fecha_proceso
--      AND estado = 'en_proceso';

--EXCEPTION WHEN OTHERS THEN

--    -- ❌ Marcar fallo
--    INSERT INTO monitoreo_resumen_inventario (
--        empresa_id,
--        almacen_id,
--        tipo_proceso,
--        fecha_desde,
--        fecha_hasta,
--        estado,
--        registros_procesados,
--        mensaje,
--        fecha_fin
--    )
--    VALUES (
--        NULL,
--        NULL,
--        'diario',
--        fecha_proceso,
--        fecha_proceso,
--        'fallido',
--        0,
--        SQLERRM,
--        timezone('America/Bogota', now())
--    );

--END;
--$$;

---- este es para informacion
--SELECT *
--FROM monitoreo_resumen_inventario
--ORDER BY creado_en DESC
--LIMIT 10;

--SELECT *
--FROM monitoreo_resumen_inventario
--WHERE fecha_desde = CURRENT_DATE
--  AND estado = 'fallido';

--SELECT *
--FROM monitoreo_resumen_inventario
--WHERE fecha_desde = CURRENT_DATE;

------------------------
---- funcion de reintento automatico 
---- ====================
--CREATE OR REPLACE FUNCTION reintentar_resumen_diario()
--RETURNS void
--LANGUAGE plpgsql
--AS $$
--DECLARE
--    registro_fallido RECORD;
--BEGIN

--    -- 🔍 Buscar último fallo del batch diario
--    SELECT *
--    INTO registro_fallido
--    FROM monitoreo_resumen_inventario
--    WHERE estado = 'fallido'
--      AND tipo_proceso = 'diario'
--    ORDER BY creado_en DESC
--    LIMIT 1;

--    -- ❌ Si no hay fallos, salir
--    IF registro_fallido IS NULL THEN
--        RETURN;
--    END IF;

--    -- ⏱ Evitar reintentos excesivos (30 min)
--    IF registro_fallido.fecha_fin IS NOT NULL
--       AND registro_fallido.fecha_fin > now() - interval '30 minutes' THEN
--        RETURN;
--    END IF;

--    -- 🟡 Marcar en proceso
--    UPDATE monitoreo_resumen_inventario
--    SET estado = 'en_proceso'
--    WHERE id = registro_fallido.id;

--    -- 🚀 Reintento seguro
--    BEGIN

--        PERFORM generar_resumen_diario_inventario();

--        -- ✅ éxito
--        UPDATE monitoreo_resumen_inventario
--        SET
--            estado = 'reintentado_exitoso',
--            mensaje = 'Reintento exitoso',
--            fecha_fin = timezone('America/Bogota', now())
--        WHERE id = registro_fallido.id;

--    EXCEPTION WHEN OTHERS THEN

--        -- ❌ fallo otra vez
--        UPDATE monitoreo_resumen_inventario
--        SET
--            estado = 'fallido',
--            mensaje = SQLERRM,
--            fecha_fin = timezone('America/Bogota', now())
--        WHERE id = registro_fallido.id;

--    END;

--END;
--$$;

---- ================================
---- funcion registrar logs_sistema
---- ============================
--CREATE OR REPLACE FUNCTION registrar_log_sistema()
--RETURNS TRIGGER
--LANGUAGE plpgsql
--SECURITY DEFINER
--AS $$

--DECLARE

--    empresa UUID;
--    registro UUID;
--    datos JSONB;

--BEGIN


--    -- Obtener empresa afectada
--    IF TG_OP = 'DELETE' THEN

--        empresa := OLD.empresa_id;
--        registro := OLD.id;
--        datos := to_jsonb(OLD);

--    ELSE

--        empresa := NEW.empresa_id;
--        registro := NEW.id;
--        datos := to_jsonb(NEW);

--    END IF;


--    INSERT INTO logs_sistema
--    (
--        empresa_id,
--        usuario_id,
--        accion,
--        detalle,
--        tabla_afectada,
--        registro_id,
--        operacion,
--        datos
--    )
--    VALUES
--    (
--        empresa,

--        -- usuario desde Supabase si existe
--        auth.uid(),

--        TG_OP,

--        'Cambio automático realizado en ' || TG_TABLE_NAME,

--        TG_TABLE_NAME,

--        registro,

--        TG_OP,

--        datos
--    );


--    RETURN COALESCE(NEW,OLD);

--END;

--$$;




---- =========================================================
---- 4. TRIGGERS
---- =========================================================

--CREATE TRIGGER on_auth_user_created
--AFTER INSERT ON auth.users
--FOR EACH ROW
--EXECUTE FUNCTION crear_usuario();

--CREATE TRIGGER trg_stock
--AFTER INSERT ON movimientos_inventario
--FOR EACH ROW
--EXECUTE FUNCTION actualizar_stock();

---- ================ trigger de logs_sistema


--CREATE TRIGGER trg_logs_empresas
--AFTER INSERT OR UPDATE OR DELETE
--ON empresas
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_usuario_empresa
--AFTER INSERT OR UPDATE OR DELETE
--ON usuarios_empresas
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_productos
--AFTER INSERT OR UPDATE OR DELETE
--ON productos
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_almacenes
--AFTER INSERT OR UPDATE OR DELETE
--ON almacenes
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_compras
--AFTER INSERT OR UPDATE OR DELETE
--ON compras
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_ventas
--AFTER INSERT OR UPDATE OR DELETE
--ON ventas
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_movimientos
--AFTER INSERT OR UPDATE OR DELETE
--ON movimientos_inventario
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_categorias
--AFTER INSERT OR UPDATE OR DELETE
--ON categorias
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_proveedores
--AFTER INSERT OR UPDATE OR DELETE
--ON proveedores
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();

--CREATE TRIGGER trg_logs_clientes
--AFTER INSERT OR UPDATE OR DELETE
--ON clientes
--FOR EACH ROW
--EXECUTE FUNCTION registrar_log_sistema();
---- =========================================================
---- 5. RLS
---- =========================================================

--ALTER TABLE empresas ENABLE ROW LEVEL SECURITY;

--CREATE POLICY empresas_select ON empresas
--USING (pertenece_empresa(id));

---- (RLS aplicado de forma consistente por tabla)
---- Por brevedad lógica: todas las tablas usan pertenece_empresa(empresa_id)
---- y políticas SELECT/INSERT/UPDATE donde aplica

---- =========================================================
---- 6. ÍNDICES
---- =========================================================

--CREATE INDEX idx_ventas_empresa_fecha ON ventas(empresa_id, created_at);
--CREATE INDEX idx_detalle_producto ON ventas_detalle(producto_id);
--CREATE INDEX idx_stock_producto ON stock_actual(producto_id);
--CREATE INDEX idx_mov_producto_fecha ON movimientos_inventario(producto_id, created_at);
--CREATE INDEX idx_logs_empresa_fecha ON logs_sistema(empresa_id, created_at);


---- 1. ALMACENES (CRÍTICO)
--CREATE INDEX idx_almacenes_empresa_activo
--ON almacenes (empresa_id, activo);

---- 2. STOCK (CRÍTICO)
--CREATE INDEX idx_stock_empresa_producto_almacen
--ON stock_actual (empresa_id, producto_id, almacen_id);

---- 3. MOVIMIENTOS (CRÍTICO)
--CREATE INDEX idx_mov_empresa_producto_fecha
--ON movimientos_inventario (empresa_id, producto_id, created_at);



--CREATE INDEX idx_resumen_diario_almacen_fecha
--ON resumen_diario_inventario (empresa_id, almacen_id, fecha);

--CREATE INDEX idx_resumen_diario_producto
--ON resumen_diario_inventario (empresa_id, producto_id);

--CREATE INDEX idx_resumen_diario_full
--ON resumen_diario_inventario (empresa_id, almacen_id, producto_id, fecha);

--CREATE INDEX idx_mov_agg
--ON movimientos_inventario (
--    empresa_id,
--    almacen_id,
--    producto_id,
--    created_at,
--    activo
--);

--CREATE INDEX idx_logs_tabla_registro
--ON logs_sistema(tabla_afectada, registro_id);




--SELECT cron.schedule(
--    'resumen_diario_inventario_job',
--    '0 4 * * *',
--    $$ SELECT generar_resumen_diario_inventario(); $$
--);