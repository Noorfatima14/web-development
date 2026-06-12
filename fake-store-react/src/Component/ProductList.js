import React, { useEffect, useState } from "react";
import axios from "axios";
import { Card, Spin, Input, Select, Button } from "antd";
import { Container, Row, Col } from "react-bootstrap";

const { Search } = Input;
const { Option } = Select;
const { Meta } = Card;

const ProductList = () => {
  const [products, setProducts] = useState([]);
  const [filtered, setFiltered] = useState([]);
  const [loading, setLoading] = useState(true);
  const [categories, setCategories] = useState([]);

  useEffect(() => {
    // Get products
    axios
      .get(process.env.REACT_APP_API_URL)
      .then((res) => {
        setProducts(res.data);
        setFiltered(res.data);
        setLoading(false);
      })
      .catch((err) => console.log(err));

    // Get categories
    axios
      .get("https://fakestoreapi.com/products/categories")
      .then((res) => setCategories(res.data))
      .catch((err) => console.log(err));
  }, []);

  // Search products
  const onSearch = (value) => {
    const result = products.filter((p) =>
      p.title.toLowerCase().includes(value.toLowerCase())
    );
    setFiltered(result);
  };

  // Filter by category
  const onCategoryChange = (value) => {
    if (value === "all") {
      setFiltered(products);
    } else {
      const result = products.filter((p) => p.category === value);
      setFiltered(result);
    }
  };

  if (loading)
    return (
      <Spin size="large" style={{ display: "block", margin: "50px auto" }} />
    );

  return (
    <Container className="mt-4">
      {/* Search & Filter */}
      <div className="d-flex justify-content-between mb-4">
        <Search
          placeholder="Search products..."
          onSearch={onSearch}
          enterButton
          style={{ width: "60%" }}
        />
        <Select
          defaultValue="all"
          style={{ width: 200 }}
          onChange={onCategoryChange}
        >
          <Option value="all">All Categories</Option>
          {categories.map((cat) => (
            <Option key={cat} value={cat}>
              {cat}
            </Option>
          ))}
        </Select>
      </div>

      {/* Product Cards */}
      <Row>
        {filtered.length > 0 ? (
          filtered.map((product) => (
            <Col md={4} className="mb-4" key={product.id}>
              <Card
                hoverable
                style={{
                  height: "100%", // full column height
                  display: "flex",
                  flexDirection: "column",
                }}
                cover={
                  <div
                    style={{
                      height: "250px",
                      display: "flex",
                      justifyContent: "center",
                      alignItems: "center",
                      background: "#f9f9f9",
                      padding: "10px",
                    }}
                  >
                    <img
                      alt={product.title}
                      src={product.image}
                      style={{ maxHeight: "220px", objectFit: "contain" }}
                    />
                  </div>
                }
              >
                <Meta
                  title={product.title}
                  description={`$${product.price}`}
                />
                <p className="mt-2" style={{ flexGrow: 1 }}>
                  {product.description.substring(0, 80)}...
                </p>

                {/* Buttons aligned at bottom */}
                <div
                  style={{
                    display: "flex",
                    justifyContent: "space-between",
                    marginTop: "auto",
                  }}
                >
                  <Button type="primary">Add to Cart</Button>
                  <Button danger>Order Now</Button>
                </div>
              </Card>
            </Col>
          ))
        ) : (
          <Col span={24} className="text-center mt-5">
            <h4>Sorry, this product is not available.</h4>
          </Col>
        )}
      </Row>
    </Container>
  );
};

export default ProductList;
