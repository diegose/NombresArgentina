El sitio [Nombres en Argentina](http://nombres.historias.datos.gob.ar)
tiene estadísticas interesantes sobre los nombres de los argentinos a lo largo de los años.

Un problema que presenta es que toma los nombres compuestos como un todo. Entonces, si yo me llamo **Diego Sebastián**,
sólo puedo saber cuántas personas se llaman exactamente igual, o **Diego** a secas, pero no cuántos llevan **Diego** en el nombre
combinado con otros.

Este proyecto hace lo siguiente:
- [Import](https://github.com/diegose/NombresArgentina/tree/master/Import) permite importar el archivo CSV que bajamos de
http://www.datos.gob.ar/dataset/nombres-personas-fisicas a una base [RavenDB](https://ravendb.net/)
- [CreateIndexes](https://github.com/diegose/NombresArgentina/tree/master/CreateIndexes) genera los índices necesarios
para las consultas
- [Query](https://github.com/diegose/NombresArgentina/tree/master/Query) permite consultar los datos para un nombre en particular

Por ahora son sólo mini utilidades de consola. Mi idea es crear una API y una visualización web bonita,
que permita combinar datos de varios nombres, etc.

Se aceptan PRs!
