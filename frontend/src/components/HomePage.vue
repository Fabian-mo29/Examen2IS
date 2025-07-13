<template>
  <div class="container py-4">
    <h1 class="text-center mb-4">Máquina de Refrescos</h1>

    <!-- Bebidas disponibles -->
    <h2 class="h4 mb-3"><strong>Bebidas disponibles</strong></h2>
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
                :max="soda.quantity"
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

    <!-- Carrito actual -->
    <h2 class="h4 mb-3"><strong>Bebidas seleccionadas</strong></h2>
    <div
      v-if="cart.length > 0"
      class="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-3 mb-4"
    >
      <div v-for="item in cart" :key="item.name" class="col">
        <div class="card h-100 bg-white">
          <div class="card-body">
            <h5 class="card-title">
              <strong>{{ item.name }}</strong>
            </h5>
            <p class="mb-1">Cantidad: {{ item.quantity }}</p>
            <p class="mb-0">Subtotal: ₡{{ item.quantity * item.price }}</p>
          </div>
        </div>
      </div>
    </div>
    <div v-else>
      <p>No hay bebidas seleccionadas</p>
    </div>

    <!-- Unidades de pago y resumen -->
    <h2 class="h4 mb-3"><strong>Unidades de pago</strong></h2>
    <div class="row">
      <div class="col-md-9">
        <div class="row g-3">
          <div
            class="col-6 col-sm-4 col-md-3 col-lg-2"
            v-for="unit in cashUnits"
            :key="unit.value"
          >
            <div class="card text-center h-100">
              <div class="card-body p-2">
                <label class="fw-bold">{{ unit.label }}</label>
                <input
                  type="number"
                  min="0"
                  v-model.number="unit.quantity"
                  class="form-control mt-1"
                />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Resumen de total de pago -->
      <div class="col-md-3">
        <div class="card p-3 bg-light h-100">
          <p class="mb-2"><strong>Precio Total:</strong> ₡{{ cartTotal }}</p>
          <p>
            <strong>Total insertado por el usuario:</strong> ₡{{
              insertedAmount
            }}
          </p>
          <button
            class="btn btn-sm flex-grow-1"
            :class="
              insertedAmount >= cartTotal && cartTotal > 0
                ? 'btn-success'
                : 'btn-secondary'
            "
            :disabled="insertedAmount < cartTotal || cartTotal === 0"
            @click="createPaymentRequest"
          >
            Comprar
          </button>
        </div>
      </div>

      <!-- Desglose de vuelto -->
      <div v-if="filteredChange.length > 0" class="mt-4 col-md-12">
        <h2 class="h5 mb-3"><strong>Desglose de vuelto</strong></h2>
        <div class="card p-3 bg-light" style="max-width: 400px">
          <ul class="mb-0">
            <p v-for="unit in filteredChange" :key="unit.value">
              <strong
                >{{ unit.quantity }} moneda{{
                  unit.quantity > 1 ? "s" : ""
                }}</strong
              >: ₡{{ unit.value }}
            </p>
          </ul>
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
      cart: [],
      cartTotal: 0,
      cashUnits: [
        { label: "₡25", value: 25, quantity: 0 },
        { label: "₡50", value: 50, quantity: 0 },
        { label: "₡100", value: 100, quantity: 0 },
        { label: "₡500", value: 500, quantity: 0 },
        { label: "₡1000", value: 1000, quantity: 0 },
      ],
      change: [],
    };
  },
  computed: {
    insertedAmount() {
      return this.cashUnits.reduce(
        (sum, unit) => sum + unit.quantity * unit.value,
        0
      );
    },
    filteredChange() {
      return this.change.filter((unit) => unit.quantity > 0);
    },
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
        console.error("Error fetching the sodas.", error);
      }
    },
    addToCart(soda) {
      if (soda.selectedQty > 0 && soda.selectedQty <= soda.quantity) {
        const existing = this.cart.find((item) => item.name === soda.name);
        if (existing) {
          existing.quantity += soda.selectedQty;
        } else {
          this.cart.push({
            name: soda.name,
            price: soda.price,
            quantity: soda.selectedQty,
          });
        }
        this.cartTotal += soda.selectedQty * soda.price;
        soda.quantity -= soda.selectedQty;
        soda.selectedQty = 0;
      }
    },
    getImage(sodaName) {
      switch (sodaName) {
        case "Pepsi":
          return "pepsi.png";
        case "Coca Cola":
          return "cocacola.png";
        case "Fanta":
          return "fanta.png";
        case "Sprite":
          return "sprite.png";
        default:
          return "default.png";
      }
    },
    createPaymentRequest() {
      const payment = {
        cashUnits: this.cashUnits
          .filter((unit) => unit.quantity > 0)
          .map(({ value, quantity }) => ({ value, quantity })),
        sodas: this.cart.map(({ name, price, quantity }) => ({
          name,
          price,
          quantity,
        })),
      };
      this.createPayment(payment);
    },
    async createPayment(payment) {
      try {
        const response = await this.$api.completePayment(payment);
        this.change = response.data;
      } catch (error) {
        console.error("Error creating payment.", error);
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
