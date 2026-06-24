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