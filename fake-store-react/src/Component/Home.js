import React, { useEffect, useState } from "react";
import axios from "axios";
import { Container, Row, Col, Card } from "react-bootstrap";

const Home = () => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    axios
      .get(process.env.REACT_APP_API_URL)
      .then((res) => {
        setProducts(res.data.slice(0, 3)); // sirf 3 trending items
      })
      .catch((err) => console.log(err));
  }, []);

  return (
    <Container className="mt-4">
      <h2 className="mb-4"> Trending Items</h2>
      <Row>
        {products.map((product) => (
          <Col md={4} className="mb-4" key={product.id}>
            <Card className="h-100 shadow-sm">
              <Card.Img
                variant="top"
                src={product.image}
                alt={product.title}
                style={{ height: "250px", objectFit: "contain" }}
              />
              <Card.Body>
                <Card.Title>{product.title}</Card.Title>
                <Card.Subtitle className="mb-2 text-muted">
                  ${product.price}
                </Card.Subtitle>
                <Card.Text>
                  {product.description.substring(0, 80)}...
                </Card.Text>
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </Container>
  );
};

export default Home;
