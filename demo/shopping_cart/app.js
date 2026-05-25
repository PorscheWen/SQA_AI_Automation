(function () {
  "use strict";

  var PRODUCTS = {
    apple: { name: "蘋果", price: 30 },
    banana: { name: "香蕉", price: 20 },
    milk: { name: "鮮奶", price: 55 }
  };

  var cart = {};

  var els = {
    cartCount: document.getElementById("cart-count"),
    cartEmptyMessage: document.getElementById("cart-empty-message"),
    cartItems: document.getElementById("cart-items"),
    cartSummary: document.getElementById("cart-summary"),
    cartSubtotal: document.getElementById("cart-subtotal"),
    cartTotal: document.getElementById("cart-total"),
    checkoutModal: document.getElementById("checkout-modal"),
    checkoutMessage: document.getElementById("checkout-message"),
    checkoutTotal: document.getElementById("checkout-total"),
    btnClearCart: document.getElementById("btn-clear-cart"),
    btnCheckout: document.getElementById("btn-checkout"),
    btnCloseModal: document.getElementById("btn-close-modal")
  };

  function formatMoney(amount) {
    return "NT$ " + amount;
  }

  function getTotalQuantity() {
    return Object.keys(cart).reduce(function (sum, id) {
      return sum + cart[id].qty;
    }, 0);
  }

  function getSubtotal() {
    return Object.keys(cart).reduce(function (sum, id) {
      return sum + cart[id].price * cart[id].qty;
    }, 0);
  }

  function addToCart(productId) {
    var product = PRODUCTS[productId];
    if (!product) {
      return;
    }

    if (!cart[productId]) {
      cart[productId] = {
        name: product.name,
        price: product.price,
        qty: 0
      };
    }

    cart[productId].qty += 1;
    renderCart();
  }

  function updateQuantity(productId, delta) {
    if (!cart[productId]) {
      return;
    }

    cart[productId].qty += delta;

    if (cart[productId].qty <= 0) {
      delete cart[productId];
    }

    renderCart();
  }

  function removeItem(productId) {
    delete cart[productId];
    renderCart();
  }

  function clearCart() {
    cart = {};
    renderCart();
  }

  function renderCart() {
    var totalQty = getTotalQuantity();
    var subtotal = getSubtotal();
    var hasItems = totalQty > 0;

    els.cartCount.textContent = totalQty + " 件";
    els.cartEmptyMessage.hidden = hasItems;
    els.cartItems.hidden = !hasItems;
    els.cartSummary.hidden = !hasItems;

    els.cartItems.innerHTML = "";

    Object.keys(cart).forEach(function (productId) {
      var item = cart[productId];
      var li = document.createElement("li");
      li.className = "cart-item";
      li.id = "cart-item-" + productId;
      li.setAttribute("data-product-id", productId);
      li.setAttribute("data-testid", "cart-item-" + productId);

      li.innerHTML =
        '<span class="cart-item-name" data-testid="cart-item-name-' + productId + '">' +
          item.name +
        "</span>" +
        '<span class="cart-item-price" data-testid="cart-item-price-' + productId + '">' +
          formatMoney(item.price * item.qty) +
        "</span>" +
        '<div class="qty-controls">' +
          '<button type="button" class="btn btn-secondary btn-icon" id="btn-qty-minus-' + productId + '" data-testid="qty-minus-' + productId + '" aria-label="減少數量">-</button>' +
          '<span class="qty-value" id="qty-value-' + productId + '" data-testid="qty-value-' + productId + '">' + item.qty + "</span>" +
          '<button type="button" class="btn btn-secondary btn-icon" id="btn-qty-plus-' + productId + '" data-testid="qty-plus-' + productId + '" aria-label="增加數量">+</button>' +
        "</div>" +
        '<button type="button" class="btn btn-secondary" id="btn-remove-' + productId + '" data-testid="remove-' + productId + '">移除</button>';

      els.cartItems.appendChild(li);

      document.getElementById("btn-qty-minus-" + productId).addEventListener("click", function () {
        updateQuantity(productId, -1);
      });
      document.getElementById("btn-qty-plus-" + productId).addEventListener("click", function () {
        updateQuantity(productId, 1);
      });
      document.getElementById("btn-remove-" + productId).addEventListener("click", function () {
        removeItem(productId);
      });
    });

    els.cartSubtotal.textContent = formatMoney(subtotal);
    els.cartTotal.textContent = formatMoney(subtotal);
  }

  function openCheckoutModal() {
    var subtotal = getSubtotal();
    els.checkoutTotal.textContent = "總金額：" + formatMoney(subtotal);
    els.checkoutModal.classList.add("is-open");
    els.checkoutModal.setAttribute("aria-hidden", "false");
  }

  function closeCheckoutModal() {
    els.checkoutModal.classList.remove("is-open");
    els.checkoutModal.setAttribute("aria-hidden", "true");
    clearCart();
  }

  document.getElementById("btn-add-apple").addEventListener("click", function () {
    addToCart("apple");
  });
  document.getElementById("btn-add-banana").addEventListener("click", function () {
    addToCart("banana");
  });
  document.getElementById("btn-add-milk").addEventListener("click", function () {
    addToCart("milk");
  });

  els.btnClearCart.addEventListener("click", clearCart);
  els.btnCheckout.addEventListener("click", function () {
    if (getTotalQuantity() === 0) {
      return;
    }
    openCheckoutModal();
  });
  els.btnCloseModal.addEventListener("click", closeCheckoutModal);
  els.checkoutModal.addEventListener("click", function (event) {
    if (event.target === els.checkoutModal) {
      closeCheckoutModal();
    }
  });
  document.addEventListener("keydown", function (event) {
    if (event.key === "Escape" && els.checkoutModal.classList.contains("is-open")) {
      closeCheckoutModal();
    }
  });

  renderCart();
})();
