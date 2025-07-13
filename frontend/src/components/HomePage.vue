<template>
  <div class="container py-4">
    <h1 class="text-center mb-4">Máquina de Refrescos</h1>

    <!-- Bebidas disponibles -->
    <h2 class="h4 mb-3">Bebidas disponibles</h2>
    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-4 g-3 mb-4">
      <div v-for="soda in sodas" :key="soda.name" class="col">
        <div class="card h-100">
          <img
            :src="`/images/${getImage(soda.name)}`"
            class="card-img-top p-3 mx-auto"
            style="height: 150px; object-fit: contain; max-width: 100%"
          />
          <div
            class="card-body text-start d-flex flex-column justify-content-between"
          >
            <h5 class="card-title">
              <strong>{{ soda.name }}</strong>
            </h5>
            <p class="card-text text-muted">In stock: {{ soda.quantity }}</p>
            <p class="fw-bold">₡{{ soda.price }}</p>

            <div class="d-flex align-items-center gap-2">
              <input
                type="number"
                min="0"
                v-model.number="soda.selectedQty"
                class="form-control form-control-sm"
                style="width: 70px"
              />
              <button
                :disabled="soda.quantity === 0"
                @click="addToCart(soda)"
                class="btn btn-sm flex-grow-1"
                :class="soda.quantity > 0 ? 'btn-success' : 'btn-secondary'"
              >
                {{ soda.quantity > 0 ? "Comprar" : "Agotado" }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "HomePage",
  data() {
    return {
      sodas: [],
      changeBreakdown: [],
    };
  },
  methods: {
    async getSodasList() {
      try {
        const response = await this.$api.getSodasList();
        this.sodas = response.data.map((soda) => ({
          ...soda,
          selectedQty: 0,
        }));
      } catch (error) {
        console.error("Error fetching the sodas", error);
      }
    },
    getImage(sodaName) {
      if (sodaName == "Pepsi") {
        return "pepsi.png";
      }
      if (sodaName == "Coca Cola") {
        return "cocacola.png";
      }
      if (sodaName == "Fanta") {
        return "fanta.png";
      }
      if (sodaName == "Sprite") {
        return "sprite.png";
      }
    },
  },
  mounted() {
    this.getSodasList();
  },
};
</script>

<style scoped>
.card {
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}
</style>
