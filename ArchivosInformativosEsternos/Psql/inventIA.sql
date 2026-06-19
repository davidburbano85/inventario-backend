---SET timezone = 'America/Bogota';

---- Tabla principal de empresas
--CREATE TABLE empresas (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID único de empresa
--  nombre TEXT NOT NULL, -- Nombre de la empresa
--  created_at TIMESTAMP DEFAULT now() -- Fecha de creación
--);

----USUARIOS POR EMPRESA (MULTI-TENANT LINK)
--CREATE TABLE usuarios_empresas (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID interno
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE, -- Empresa
--  usuario_id UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE, -- Usuario Supabase
--  rol TEXT NOT NULL DEFAULT 'usuario', -- Rol dentro de la empresa
--  created_at TIMESTAMP DEFAULT now() -- Fecha creación
--);

--CREATE TABLE usuarios (
--  id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
--  nombre TEXT,
--  telefono TEXT,
--  created_at TIMESTAMPTZ DEFAULT now()
--);

----almacenes
--CREATE TABLE almacenes (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID almacén
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE, -- Empresa dueña
--  nombre TEXT NOT NULL, -- Nombre almacén
--  ubicacion TEXT, -- Ubicación física
--  created_at TIMESTAMP DEFAULT now() -- Creación
--);

----categorias
--CREATE TABLE categorias (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID categoría
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE, -- Empresa
--  nombre TEXT NOT NULL, -- Nombre categoría
--  created_at TIMESTAMP DEFAULT now() -- Creación
--);

----productos
--CREATE TABLE productos (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID producto
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE, -- Empresa
--  categoria_id UUID REFERENCES categorias(id), -- Categoría
--  nombre TEXT NOT NULL, -- Nombre producto
--  codigo_sku TEXT, -- SKU único
--  precio_venta NUMERIC(12,2) NOT NULL, -- Precio venta
--  precio_compra NUMERIC(12,2), -- Precio compra
--  activo BOOLEAN DEFAULT true, -- Estado producto
--  created_at TIMESTAMP DEFAULT now() -- Creación
--);

----stock actual materializado
--CREATE TABLE stock_actual (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID stock
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE, -- Empresa
--  producto_id UUID NOT NULL REFERENCES productos(id) ON DELETE CASCADE, -- Producto
--  almacen_id UUID NOT NULL REFERENCES almacenes(id) ON DELETE CASCADE, -- Almacén
--  cantidad NUMERIC DEFAULT 0, -- Stock actual
--  updated_at TIMESTAMP DEFAULT now() -- Última actualización
--);

----movimientos inventario
--CREATE TABLE movimientos_inventario (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID movimiento
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE, -- Empresa
--  producto_id UUID NOT NULL REFERENCES productos(id), -- Producto
--  almacen_id UUID NOT NULL REFERENCES almacenes(id), -- Almacén
--  usuario_id UUID REFERENCES auth.users(id), -- Usuario que ejecuta
--  tipo TEXT NOT NULL, -- entrada | salida | ajuste
--  cantidad NUMERIC NOT NULL, -- Cantidad
--  motivo TEXT, -- Motivo movimiento
--  created_at TIMESTAMP DEFAULT now() -- Fecha
--);

----proveedores
--CREATE TABLE proveedores (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID proveedor
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE, -- Empresa
--  nombre TEXT NOT NULL, -- Nombre proveedor
--  contacto TEXT, -- Contacto
--  created_at TIMESTAMP DEFAULT now() -- Creación
--);

----clientes
--CREATE TABLE clientes (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID cliente
--  empresa_id UUID NOT NULL REFERENCES empresas(id) ON DELETE CASCADE, -- Empresa
--  nombre TEXT NOT NULL, -- Nombre cliente
--  contacto TEXT, -- Contacto
--  created_at TIMESTAMP DEFAULT now() -- Creación
--);

----compras
--CREATE TABLE compras (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID compra
--  empresa_id UUID NOT NULL REFERENCES empresas(id), -- Empresa
--  proveedor_id UUID REFERENCES proveedores(id), -- Proveedor
--  total NUMERIC(12,2), -- Total compra
--  created_at TIMESTAMP DEFAULT now() -- Fecha
--);

----detalled e compras
--CREATE TABLE compras_detalle (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID detalle
--  empresa_id UUID NOT NULL REFERENCES empresas(id), -- Empresa
--  compra_id UUID REFERENCES compras(id) ON DELETE CASCADE, -- Compra
--  producto_id UUID REFERENCES productos(id), -- Producto
--  cantidad NUMERIC NOT NULL, -- Cantidad
--  precio NUMERIC NOT NULL -- Precio unitario
--);

----ventas
--CREATE TABLE ventas (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID venta
--  empresa_id UUID NOT NULL REFERENCES empresas(id), -- Empresa
--  cliente_id UUID REFERENCES clientes(id), -- Cliente
--  total NUMERIC(12,2), -- Total venta
--  created_at TIMESTAMP DEFAULT now() -- Fecha
--);

----detalleventas
--CREATE TABLE ventas_detalle (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID detalle venta
--  empresa_id UUID NOT NULL REFERENCES empresas(id), -- Empresa
--  venta_id UUID REFERENCES ventas(id) ON DELETE CASCADE, -- Venta
--  producto_id UUID REFERENCES productos(id), -- Producto
--  cantidad NUMERIC NOT NULL, -- Cantidad
--  precio NUMERIC NOT NULL -- Precio unitario
--);

----resumen diario inventario
--CREATE TABLE resumen_diario_inventario (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID resumen
--  empresa_id UUID NOT NULL REFERENCES empresas(id), -- Empresa
--  producto_id UUID REFERENCES productos(id), -- Producto
--  fecha DATE NOT NULL, -- Día
--  total_ventas NUMERIC DEFAULT 0, -- Ventas del día
--  total_compras NUMERIC DEFAULT 0 -- Compras del día
--);

----logs sistema
--CREATE TABLE logs_sistema (
--  id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- ID log
--  empresa_id UUID NOT NULL REFERENCES empresas(id), -- Empresa
--  usuario_id UUID REFERENCES auth.users(id), -- Usuario
--  accion TEXT, -- Acción realizada
--  detalle TEXT, -- Detalle
--  created_at TIMESTAMP DEFAULT now() -- Fecha
--);

----funcion de seguridad multitenat
--CREATE OR REPLACE FUNCTION pertenece_empresa(emp_id UUID)
--RETURNS BOOLEAN AS $$
--BEGIN
--  RETURN EXISTS (
--    SELECT 1
--    FROM usuarios_empresas ue
--    WHERE ue.usuario_id = auth.uid() -- Usuario autenticado
--    AND ue.empresa_id = emp_id -- Empresa consultada
--  );
--END;
--$$ LANGUAGE plpgsql SECURITY DEFINER;

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
--    SELECT 1
--    FROM usuarios_empresas ue
--    WHERE ue.usuario_id = auth.uid()
--      AND ue.empresa_id = emp_id
--  );
--END;
--$$;

----***********         RLS row level security*********
----empresas
--ALTER TABLE empresas ENABLE ROW LEVEL SECURITY;


--CREATE POLICY "select_empresas"
--ON empresas FOR SELECT
--USING (pertenece_empresa(id));

----usuarios empresas
--ALTER TABLE usuarios_empresas ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_usuarios_empresa"
--ON usuarios_empresas FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "ue_select"
--ON usuarios_empresas
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

----USUARIOS

--create  or replace function public.handle_new_user()
--returns trigger as $$
--begin
--  insert into public.profiles (id, nombre, telefono)
--  values (
--    new.id,
--    new.raw_user_meta_data ->> 'nombre',
--    new.raw_user_meta_data ->> 'telefono'
--  );
--  return new;
--end;
--$$ language plpgsql;

--create trigger on_auth_user_create
--after insert on auth.users
--for each row
--execute function public.handle_new_user();

--create or replace function public.crear_usuario()
--returns trigger
--language plpgsql
--security definer
--as $$
--begin
--  insert into public.usuarios (id)
--  values (new.id);

--  return new;
--end;
--$$;





--ALTER TABLE usuarios ENABLE ROW LEVEL SECURITY;


--CREATE POLICY "usuarios_select_own"
--ON usuarios
--FOR SELECT
--USING (id = auth.uid());

--CREATE POLICY "usuarios_insert_own"
--ON usuarios
--FOR INSERT
--WITH CHECK (id = auth.uid());

--CREATE POLICY "usuarios_update_own"
--ON usuarios
--FOR UPDATE
--USING (id = auth.uid())
--WITH CHECK (id = auth.uid());
----productos
--ALTER TABLE productos ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "productos_select"
--ON productos FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "productos_insert"
--ON productos FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

----sctok actual 
--ALTER TABLE stock_actual ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "stock_insert"
--ON stock_actual
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "stock_select"
--ON stock_actual FOR SELECT
--USING (pertenece_empresa(empresa_id));

----VENTAS
--ALTER TABLE ventas ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "ventas_select"
--ON ventas FOR SELECT
--USING (pertenece_empresa(empresa_id));

----MOVIMIENTOS INVENTARIO
--ALTER TABLE movimientos_inventario ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "mov_select"
--ON movimientos_inventario FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "mov_insert"
--ON movimientos_inventario FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));
----ALMACENES
--ALTER TABLE almacenes ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "almacenes_select"
--ON almacenes
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "almacenes_insert"
--ON almacenes
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));


----CATEGORIAS
--ALTER TABLE categorias ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "categorias_select"
--ON categorias
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "categorias_insert"
--ON categorias
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));


----CLIENTES

--ALTER TABLE clientes ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "clientes_select"
--ON clientes
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "clientes_insert"
--ON clientes
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

----PROOVEDORES
--ALTER TABLE proveedores ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "proveedores_select"
--ON proveedores
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "proveedores_insert"
--ON proveedores
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

---- COMPRAS
--ALTER TABLE compras ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "compras_select"
--ON compras
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "compras_insert"
--ON compras
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

----COMPRAS DETALLE
--ALTER TABLE compras_detalle ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "compras_detalle_select"
--ON compras_detalle
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "compras_detalle_insert"
--ON compras_detalle
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

----VENTAS  
--ALTER TABLE ventas ENABLE ROW LEVEL SECURITY;
--CREATE POLICY "ventas_insert"
--ON ventas
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));
----VENTAS DETALLE

--ALTER TABLE ventas_detalle ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "ventas_detalle_select"
--ON ventas_detalle
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "ventas_detalle_insert"
--ON ventas_detalle
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

----RESUMEN movimientos_inventario
--ALTER TABLE resumen_diario_inventario ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "resumen_select"
--ON resumen_diario_inventario
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "resumen_insert"
--ON resumen_diario_inventario
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

----LOGS

--ALTER TABLE logs_sistema ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "logs_select"
--ON logs_sistema
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "logs_insert"
--ON logs_sistema
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));
----********* * TRIGGERS CRÍTICOS ********

---- **************** DATOS USUARIO **************
--CREATE OR REPLACE FUNCTION public.crear_usuario()
--RETURNS TRIGGER AS $$
--BEGIN
--  INSERT INTO public.usuarios (id)
--  VALUES (NEW.id);

--  RETURN NEW;
--END;
--$$ LANGUAGE plpgsql SECURITY DEFINER;

----actualizamos 
--CREATE OR REPLACE FUNCTION public.crear_usuario()
--RETURNS TRIGGER AS $$
--BEGIN
--  -- aqui se reemplaza esto por que el INSERT puede fallar por RLS
--  -- por esto usamos SECURITY DEFINER para evitar bloqueo de políticas
--  INSERT INTO public.usuarios (id)
--  VALUES (NEW.id);

--  RETURN NEW;
--END;
--$$ LANGUAGE plpgsql SECURITY DEFINER;

----**************TRIGGER DATOS USUARIO **********************

--CREATE TRIGGER on_auth_user_created
--AFTER INSERT ON auth.users
--FOR EACH ROW
--EXECUTE FUNCTION public.crear_usuario();
--DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;--eliminamos este codigo_sku

----actualizamos

--CREATE TRIGGER on_auth_user_created
--AFTER INSERT ON auth.users
--FOR EACH ROW
--EXECUTE FUNCTION public.crear_usuario();
----ACTUALIZAR STOCK AUTOMÁTICO 

--CREATE OR REPLACE FUNCTION actualizar_stock()
--RETURNS TRIGGER AS $$
--BEGIN
--  IF NEW.tipo = 'entrada' THEN
--    UPDATE stock_actual
--    SET cantidad = cantidad + NEW.cantidad
--    WHERE producto_id = NEW.producto_id
--    AND almacen_id = NEW.almacen_id;
--  ELSE
--    UPDATE stock_actual
--    SET cantidad = cantidad - NEW.cantidad
--    WHERE producto_id = NEW.producto_id
--    AND almacen_id = NEW.almacen_id;
--  END IF;

--  RETURN NEW;
--END;
--$$ LANGUAGE plpgsql;

--CREATE TRIGGER trg_stock
--AFTER INSERT ON movimientos_inventario
--FOR EACH ROW
--EXECUTE FUNCTION actualizar_stock();


----LOG AUTOMÁTICO
--CREATE OR REPLACE FUNCTION log_accion()
--RETURNS TRIGGER AS $$
--BEGIN
--  INSERT INTO logs_sistema(empresa_id, usuario_id, accion, detalle)
--  VALUES (NEW.empresa_id, auth.uid(), TG_OP, 'Cambio en tabla');
--  RETURN NEW;
--END;
--$$ LANGUAGE plpgsql;

---- ÍNDICES PARA ANALÍTICA
--CREATE INDEX idx_ventas_empresa_fecha ON ventas(empresa_id, created_at); -- ventas mensuales
--CREATE INDEX idx_detalle_producto ON ventas_detalle(producto_id); -- ranking productos
--CREATE INDEX idx_stock_producto ON stock_actual(producto_id); -- stock rápido
--CREATE INDEX idx_mov_producto_fecha ON movimientos_inventario(producto_id, created_at); --rotacion



----***************** CAMBIOS EN LA BD *****************

--ALTER TABLE usuarios ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "usuarios_select_propio"
--ON usuarios
--FOR SELECT
--USING (id = auth.uid());

--CREATE POLICY "usuarios_insert_propio"
--ON usuarios
--FOR INSERT
--WITH CHECK (id = auth.uid());

--CREATE POLICY "usuarios_update_propio"
--ON usuarios
--FOR UPDATE
--USING (id = auth.uid())
--WITH CHECK (id = auth.uid());
---- 1. Agregar updated_at
--ALTER TABLE empresas
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--CREATE POLICY "usuarios_insert_system"
--ON usuarios
--FOR INSERT
--WITH CHECK (true);

---- 2. Migrar created_at a TIMESTAMPTZ (si aún es TIMESTAMP)
--ALTER TABLE empresas
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

---- 3. Ajustar default de created_at a UTC (opcional pero recomendado)
--ALTER TABLE empresas
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());
---- Evita nombres vacíos o solo espacios
--ALTER TABLE empresas
--ADD CONSTRAINT empresas_nombre_check CHECK (length(trim(nombre)) > 0);


--ALTER TABLE usuarios_empresas
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());


--ALTER TABLE usuarios_empresas
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE usuarios_empresas
--ADD CONSTRAINT usuarios_empresas_unique UNIQUE (empresa_id, usuario_id);


--ALTER TABLE usuarios_empresas
--ADD CONSTRAINT usuarios_empresas_rol_check
--CHECK (rol IN ('usuario', 'admin', 'superadmin'));


--ALTER TABLE almacenes
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE almacenes
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE almacenes
--ADD CONSTRAINT almacenes_empresa_nombre_unique
--UNIQUE (empresa_id, nombre);

--ALTER TABLE almacenes ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_almacenes"
--ON almacenes
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_almacenes"
--ON almacenes
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_almacenes"
--ON almacenes
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));


--ALTER TABLE categorias
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE categorias
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE categorias
--ADD CONSTRAINT categorias_empresa_nombre_unique
--UNIQUE (empresa_id, nombre);

--ALTER TABLE categorias
--ADD CONSTRAINT categorias_nombre_check
--CHECK (length(trim(nombre)) > 0);

--ALTER TABLE categorias ENABLE ROW LEVEL SECURITY;
--CREATE POLICY "select_categorias"
--ON categorias
--FOR SELECT
--USING (pertenece_empresa(empresa_id));
--CREATE POLICY "insert_categorias"
--ON categorias
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_categorias"
--ON categorias
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));





--ALTER TABLE productos
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE productos
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE productos
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE productos
--ADD CONSTRAINT productos_empresa_sku_unique
--UNIQUE (empresa_id, codigo_sku);

--ALTER TABLE productos
--ADD CONSTRAINT productos_precio_venta_check
--CHECK (precio_venta >= 0);

--ALTER TABLE productos
--ADD CONSTRAINT productos_precio_compra_check
--CHECK (precio_compra IS NULL OR precio_compra >= 0);

--ALTER TABLE productos
--ADD CONSTRAINT productos_nombre_check
--CHECK (length(trim(nombre)) > 0);




--ALTER TABLE productos ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_productos"
--ON productos
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_productos"
--ON productos
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_productos"
--ON productos
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));

--ALTER TABLE stock_actual
--ALTER COLUMN updated_at TYPE TIMESTAMPTZ
--USING updated_at AT TIME ZONE 'UTC';

--ALTER TABLE stock_actual
--ALTER COLUMN updated_at SET DEFAULT timezone('utc', now());

--ALTER TABLE stock_actual
--ADD CONSTRAINT stock_actual_unique
--UNIQUE (empresa_id, producto_id, almacen_id);

--ALTER TABLE stock_actual
--ADD CONSTRAINT stock_actual_cantidad_check
--CHECK (cantidad >= 0);



--ALTER TABLE stock_actual ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_stock_actual"
--ON stock_actual
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_stock_actual"
--ON stock_actual
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_stock_actual"
--ON stock_actual
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));


--ALTER TABLE movimientos_inventario
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE movimientos_inventario
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE movimientos_inventario
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE movimientos_inventario
--ADD CONSTRAINT movimientos_tipo_check
--CHECK (tipo IN ('entrada', 'salida', 'ajuste'));

--ALTER TABLE movimientos_inventario
--ADD CONSTRAINT movimientos_cantidad_check
--CHECK (cantidad > 0);



--ALTER TABLE movimientos_inventario ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_movimientos_inventario"
--ON movimientos_inventario
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_movimientos_inventario"
--ON movimientos_inventario
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_movimientos_inventario"
--ON movimientos_inventario
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));

--ALTER TABLE proveedores
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE proveedores
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE proveedores
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE proveedores
--ADD CONSTRAINT proveedores_empresa_nombre_unique
--UNIQUE (empresa_id, nombre);

--ALTER TABLE proveedores
--ADD CONSTRAINT proveedores_nombre_check
--CHECK (length(trim(nombre)) > 0);



--ALTER TABLE proveedores ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_proveedores"
--ON proveedores
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_proveedores"
--ON proveedores
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_proveedores"
--ON proveedores
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));

--ALTER TABLE clientes
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE clientes
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE clientes
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE clientes
--ADD CONSTRAINT clientes_empresa_nombre_unique
--UNIQUE (empresa_id, nombre);

--ALTER TABLE clientes
--ADD CONSTRAINT clientes_nombre_check
--CHECK (length(trim(nombre)) > 0);



--ALTER TABLE clientes ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_clientes"
--ON clientes
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_clientes"
--ON clientes
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_clientes"
--ON clientes
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));


--ALTER TABLE compras
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE compras
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE compras
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE compras
--ALTER COLUMN total SET NOT NULL;

--ALTER TABLE compras
--ADD CONSTRAINT compras_total_check
--CHECK (total >= 0);



--ALTER TABLE compras ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_compras"
--ON compras
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_compras"
--ON compras
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_compras"
--ON compras
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));



--ALTER TABLE compras_detalle
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE compras_detalle
--ADD CONSTRAINT compras_detalle_cantidad_check
--CHECK (cantidad > 0);

--ALTER TABLE compras_detalle
--ADD CONSTRAINT compras_detalle_precio_check
--CHECK (precio >= 0);

--ALTER TABLE compras_detalle
--ALTER COLUMN compra_id SET NOT NULL;

--ALTER TABLE compras_detalle
--ALTER COLUMN producto_id SET NOT NULL;

--ALTER TABLE compras_detalle
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE compras_detalle
--ADD CONSTRAINT compras_detalle_cantidad_check
--CHECK (cantidad > 0);

--ALTER TABLE compras_detalle
--ADD CONSTRAINT compras_detalle_precio_check
--CHECK (precio >= 0);

--ALTER TABLE compras_detalle
--ALTER COLUMN compra_id SET NOT NULL;

--ALTER TABLE compras_detalle
--ALTER COLUMN producto_id SET NOT NULL;

--ALTER TABLE compras_detalle
--ALTER COLUMN empresa_id SET NOT NULL;




--ALTER TABLE compras_detalle ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_compras_detalle"
--ON compras_detalle
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_compras_detalle"
--ON compras_detalle
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_compras_detalle"
--ON compras_detalle
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));




--ALTER TABLE ventas
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE ventas
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE ventas
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE ventas
--ALTER COLUMN total SET NOT NULL;

--ALTER TABLE ventas
--ADD CONSTRAINT ventas_total_check
--CHECK (total >= 0);

--ALTER TABLE ventas
--ALTER COLUMN empresa_id SET NOT NULL;




--ALTER TABLE ventas ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_ventas"
--ON ventas
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_ventas"
--ON ventas
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_ventas"
--ON ventas
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));



--ALTER TABLE ventas_detalle
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE ventas_detalle
--ADD CONSTRAINT ventas_detalle_cantidad_check
--CHECK (cantidad > 0);

--ALTER TABLE ventas_detalle
--ADD CONSTRAINT ventas_detalle_precio_check
--CHECK (precio >= 0);

--ALTER TABLE ventas_detalle
--ALTER COLUMN venta_id SET NOT NULL;

--ALTER TABLE ventas_detalle
--ALTER COLUMN producto_id SET NOT NULL;

--ALTER TABLE ventas_detalle
--ALTER COLUMN empresa_id SET NOT NULL;




--ALTER TABLE ventas_detalle ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_ventas_detalle"
--ON ventas_detalle
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_ventas_detalle"
--ON ventas_detalle
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_ventas_detalle"
--ON ventas_detalle
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));



--ALTER TABLE resumen_diario_inventario
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());

--ALTER TABLE resumen_diario_inventario
--ALTER COLUMN total_ventas SET DEFAULT 0;

--ALTER TABLE resumen_diario_inventario
--ALTER COLUMN total_compras SET DEFAULT 0;

--ALTER TABLE resumen_diario_inventario
--ADD CONSTRAINT resumen_ventas_check
--CHECK (total_ventas >= 0);

--ALTER TABLE resumen_diario_inventario
--ADD CONSTRAINT resumen_compras_check
--CHECK (total_compras >= 0);

--ALTER TABLE resumen_diario_inventario
--ADD CONSTRAINT resumen_diario_unique
--UNIQUE (empresa_id, producto_id, fecha);

--ALTER TABLE resumen_diario_inventario
--ALTER COLUMN empresa_id SET NOT NULL;

--ALTER TABLE resumen_diario_inventario
--ALTER COLUMN fecha SET NOT NULL;




--ALTER TABLE resumen_diario_inventario ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_resumen_diario_inventario"
--ON resumen_diario_inventario
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_resumen_diario_inventario"
--ON resumen_diario_inventario
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_resumen_diario_inventario"
--ON resumen_diario_inventario
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));



--ALTER TABLE logs_sistema
--ALTER COLUMN created_at TYPE TIMESTAMPTZ
--USING created_at AT TIME ZONE 'UTC';

--ALTER TABLE logs_sistema
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE logs_sistema
--ALTER COLUMN created_at SET DEFAULT timezone('utc', now());

--ALTER TABLE logs_sistema
--ADD COLUMN updated_at TIMESTAMPTZ DEFAULT timezone('utc', now());


--ALTER TABLE logs_sistema
--ALTER COLUMN usuario_id SET NOT NULL;



--ALTER TABLE logs_sistema ENABLE ROW LEVEL SECURITY;

--CREATE POLICY "select_logs_sistema"
--ON logs_sistema
--FOR SELECT
--USING (pertenece_empresa(empresa_id));

--CREATE POLICY "insert_logs_sistema"
--ON logs_sistema
--FOR INSERT
--WITH CHECK (pertenece_empresa(empresa_id));

--CREATE POLICY "update_logs_sistema"
--ON logs_sistema
--FOR UPDATE
--USING (pertenece_empresa(empresa_id))
--WITH CHECK (pertenece_empresa(empresa_id));



--CREATE INDEX idx_logs_empresa_fecha ON logs_sistema(empresa_id, created_at);

--CREATE OR REPLACE FUNCTION pertenece_empresa(emp_id UUID)
--RETURNS BOOLEAN AS $$
--BEGIN
--  IF auth.uid() IS NULL THEN
--    RETURN FALSE;
--  END IF;

--  RETURN EXISTS (
--    SELECT 1
--    FROM usuarios_empresas ue
--    WHERE ue.usuario_id = auth.uid()
--      AND ue.empresa_id = emp_id
--  );
--END;
--$$ LANGUAGE plpgsql SECURITY DEFINER;


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
--    SELECT 1
--    FROM usuarios_empresas ue
--    WHERE ue.usuario_id = auth.uid()
--      AND ue.empresa_id = emp_id
--  );
--END;
--$$;





--ALTER TABLE almacenes ENABLE ROW LEVEL SECURITY;
--ALTER TABLE categorias ENABLE ROW LEVEL SECURITY;
--ALTER TABLE clientes ENABLE ROW LEVEL SECURITY;
--ALTER TABLE proveedores ENABLE ROW LEVEL SECURITY;
--ALTER TABLE compras ENABLE ROW LEVEL SECURITY;
--ALTER TABLE compras_detalle ENABLE ROW LEVEL SECURITY;
--ALTER TABLE ventas_detalle ENABLE ROW LEVEL SECURITY;
--ALTER TABLE resumen_diario_inventario ENABLE ROW LEVEL SECURITY;

--select * from usuarios;
---cambio de horario a colombia
--DO $$
--DECLARE
--    r RECORD;
--BEGIN
--    FOR r IN
--        SELECT table_schema, table_name
--        FROM information_schema.columns
--        WHERE column_name = 'created_at'
--          AND table_schema = 'public'
--    LOOP
--        EXECUTE format(
--            'ALTER TABLE %I.%I ALTER COLUMN created_at SET DEFAULT timezone(''America/Bogota'', now())',
--            r.table_schema,
--            r.table_name
--        );
--    END LOOP;
--END $$;

--DO $$
--DECLARE
--    r RECORD;
--BEGIN
--    FOR r IN
--        SELECT table_schema, table_name
--        FROM information_schema.columns
--        WHERE column_name = 'updated_at'
--          AND table_schema = 'public'
--    LOOP
--        EXECUTE format(
--            'ALTER TABLE %I.%I ALTER COLUMN updated_at SET DEFAULT timezone(''America/Bogota'', now())',
--            r.table_schema,
--            r.table_name
--        );
--    END LOOP;
--END $$;






